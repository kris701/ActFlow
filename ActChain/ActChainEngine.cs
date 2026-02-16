using ActChain.Models;
using ActChain.Models.Actions;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ActChain
{
	public class ActChainEngine : IActChainEngine
	{
		public List<IActionExecutor> Executors { get; } = new List<IActionExecutor>();
		public List<ActScriptState> ActiveScripts { get; } = new List<ActScriptState>();
		public int StageLimiter { get; set; } = 100;
		public int RemoveDelay { get; set; } = 60000;

		private readonly Dictionary<ServiceKey, IActionExecutor> _serviceCache = new Dictionary<ServiceKey, IActionExecutor>();
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);
		private readonly ILogger _logger;

		public ActChainEngine(List<IActionExecutor> executors, ILogger logger)
		{
			_logger = logger;
			Executors = executors;
			foreach (var executor in executors)
			{
				var tpy = executor.GetType();
				var baseType = tpy.BaseType;
				var typeArg = baseType?.GenericTypeArguments[0].Name;
				var key = new ServiceKey(executor.ID, typeArg);
				if (!_serviceCache.ContainsKey(key))
					_serviceCache.Add(key, executor);
				else
					_logger.LogWarning($"Multiple services with the key '{key}' exists! Ignoring all but the first...");
			}
		}

		public async Task<ActScriptState> RunScript(ActScript item)
		{
			var newState = new ActScriptState(item);
			return await RunScript(newState);
		}

		public async Task<ActScriptState> RunScript(ActScriptState item)
		{
			ActiveScripts.Add(item);
			if (item.Stage == 0)
				item.AppendToLog("Chain started!");
			else
				item.AppendToLog("Chain continued...");

			if (item.Stage < 0)
				item.Stage = 0;

			int stageCount = item.Stage;
			item.Status = ScriptStatus.Running;
			item.StartedAt = DateTime.UtcNow;
			await item.Update();
			while (item.Stage < item.Script.Stages.Count)
			{
				item.AppendToLog();
				item.AppendToLog($"\tStarting stage: {item.Stage}");
				if (item.Status == ScriptStatus.Canceled)
				{
					item.AppendToLog($"\tManual cancellation requested.");
					break;
				}

				try
				{
					var stage = item.Script.Stages[item.Stage].Clone();
					item.AppendToLog($"\tInitializing action {stage.Name}");

					stage = ApplyContexts(item, stage);

					item.AppendToLog($"\tExecuting action");
					stage.Token = item.TokenSource.Token;
					if (item.Status != ScriptStatus.Canceled && stage is IAwaitInputAction)
						item.Status = ScriptStatus.AwaitingInput;
					await item.Update();
					var executionResult = await ExecuteActionAsync(stage, item);
					if (item.Status != ScriptStatus.Canceled)
						item.Status = ScriptStatus.Running;
					if (item.TokenSource.IsCancellationRequested)
						break;
					item.AppendToLog($"\tResulting action context is a {executionResult.Context.GetType()}");
					item.AppendToLog($"\tResulting action context content is {executionResult.Context.ToString()}");

					InsertResultIntoContextStore(item, stage.Name, executionResult);

					item.Stage = FindNextStageIndex(item, executionResult);
				}
				catch (Exception ex)
				{
					item.AppendToLogError($"Message: {ex.Message}");
					item.AppendToLogError($"Trace: {ex.StackTrace}");
					item.Status = ScriptStatus.Failed;
					break;
				}

				stageCount++;
				if (stageCount > StageLimiter)
				{
					item.AppendToLogError($"Chain exceded the stage limiter (of {StageLimiter} steps)! This most likely means something broke or you somehow managed to make the Chain loop.");
					item.Status = ScriptStatus.Canceled;
					break;
				}
			}
			if (item.Status == ScriptStatus.Running)
				item.Status = ScriptStatus.Succeeded;
			item.EndedAt = DateTime.UtcNow;
			item.AppendToLog($"Chain ended!");

			await item.Update();

			await ProcessCompleted(item);
			return item;
		}

		private IAIAction ApplyContexts(ActScriptState state, IAIAction stage)
		{
			var text = JsonSerializer.Serialize(stage);
			var matches = _variableRegex.Matches(text);
			foreach (Match match in matches)
			{
				var key = match.Groups[1].Value.ToLower();
				if (state.ContextStore.Keys.Contains(key))
					text = text.Replace(match.Groups[0].Value, state.ContextStore[key]);
				else
					throw new Exception($"Variable '{key}' was not found in the current state!");
			}
			var deStage = JsonSerializer.Deserialize<IAIAction>(text);
			if (deStage == null)
				throw new ArgumentNullException("Could not deserialize the action stage!");
			return deStage;
		}

		private int FindNextStageIndex(ActScriptState state, ExecutorResult executionResult)
		{
			var nextStageIndex = state.Stage + 1;
			if (executionResult.TargetAction != "")
			{
				state.AppendToLog($"\tNext target action is named {executionResult.TargetAction}");
				nextStageIndex = state.Script.Stages.FindIndex(x => x.Name == executionResult.TargetAction);
				if (nextStageIndex == -1)
					throw new Exception($"Could not find a action named '{executionResult.TargetAction}'");
				if (state.Script.Stages.Count(x => x.Name == executionResult.TargetAction) > 1)
					state.AppendToLog($"Warning, multiple actions with the name '{executionResult.TargetAction}' found! Using the first one...");
			}
			return nextStageIndex;
		}

		private void InsertResultIntoContextStore(ActScriptState state, string actionName, ExecutorResult executionResult)
		{
			var values = executionResult.Context.GetContextValues();
			foreach (var valueKey in values.Keys)
				state.AddContext($"{actionName}.{valueKey}".ToLower(), values[valueKey]);
		}

		private async Task ProcessCompleted(ActScriptState state)
		{
			if (!state.IsProcessingUserInput)
				await state.Complete();

			if (state.TokenSource.IsCancellationRequested)
				ActiveScripts.Remove(state);
			else
			{
				state.AppendToLog($"Removing from Active List in {RemoveDelay}ms");
				await state.Update();
				Task.Run(async () =>
				{
					await Task.Delay(RemoveDelay);
					ActiveScripts.Remove(state);
				});
			}
		}

		public async Task CancelScript(Guid id)
		{
			var target = ActiveScripts.FirstOrDefault(x => x.ID == id);
			if (target != null)
			{
				target.AppendToLog("Cancellation requested");
				target.TokenSource.Cancel();
				while (ActiveScripts.Contains(target))
					await Task.Delay(500);
				target.Status = ScriptStatus.Canceled;
				await target.Update();
			}
		}

		public async Task CancelAll()
		{
			foreach (var item in ActiveScripts)
				await CancelScript(item.ID);
		}

		private async Task<ExecutorResult> ExecuteActionAsync(IAIAction act, ActScriptState state)
		{
			var targetKey = new ServiceKey(act.ExecutorID, act.GetType().Name);
			if (_serviceCache.ContainsKey(targetKey))
			{
				var executor = _serviceCache[targetKey];
				return await ExecuteActionAsync((dynamic)executor, (dynamic)act, state);
			}
			throw new Exception($"Unknown target action executor '{targetKey}'! This probably means that the backend have not been set up to accept this type of action!");
		}

		private async Task<ExecutorResult> ExecuteActionAsync<T>(BaseActionExecutor<T> executor, T act, ActScriptState state) where T : IAIAction
		{
			return await executor.ExecuteActionAsync(act, state);
		}

		public async Task<ActScriptState> UserInput(Guid chainID, ActScriptState newState)
		{
			var state = ActiveScripts.FirstOrDefault(x => x.ID == chainID);
			if (state != null)
			{
				state.AppendToLog("Applying user input");
				var combinedState = new ActScriptState(state);
				combinedState.Script = newState.Script;
				state.IsProcessingUserInput = true;
				await CancelScript(state.ID);
				combinedState.Status = ScriptStatus.NotStarted;
				combinedState.AppendToLog("Rerunning chain...");
				await combinedState.Update();
				RunScript(combinedState);
				return combinedState;
			}
			throw new Exception($"No chain log item with the id '{chainID}' found!");
		}
	}
}
