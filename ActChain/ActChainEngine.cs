using ActChain.Models;
using ActChain.Models.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ActChain
{
	public class ActChainEngine : IActChainEngine
	{
		public List<IWorker> Workers { get; } = new List<IWorker>();
		public List<ActScriptState> ActiveScripts { get; } = new List<ActScriptState>();
		public int StageLimiter { get; set; } = 100;
		public int RemoveDelay { get; set; } = 60000;

		private readonly Dictionary<ServiceKey, IWorker> _serviceCache = new Dictionary<ServiceKey, IWorker>();
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);
		private readonly ILogger _logger;

		public ActChainEngine(List<IWorker> workers, ILogger logger)
		{
			_logger = logger;
			Workers = workers;
			foreach (var worker in workers)
			{
				var tpy = worker.GetType();
				var baseType = tpy.BaseType;
				var typeArg = baseType?.GenericTypeArguments[0].Name;
				var key = new ServiceKey(worker.ID, typeArg);
				if (!_serviceCache.ContainsKey(key))
					_serviceCache.Add(key, worker);
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
					item.AppendToLog($"\tInitializing activity {stage.Name}");

					stage = ApplyContexts(item, stage);

					item.AppendToLog($"\tExecuting activity");
					if (item.Status != ScriptStatus.Canceled && stage is IAwaitInputActivity)
						item.Status = ScriptStatus.AwaitingInput;
					await item.Update();
					var executionResult = await ExecuteActivityAsync(stage, item, item.TokenSource.Token);
					if (item.Status != ScriptStatus.Canceled)
						item.Status = ScriptStatus.Running;
					if (item.TokenSource.IsCancellationRequested)
						break;
					item.AppendToLog($"\tResulting activity context is a {executionResult.Context.GetType()}");
					item.AppendToLog($"\tResulting activity context content is {executionResult.Context.ToString()}");

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

		private IActivity ApplyContexts(ActScriptState state, IActivity stage)
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
			var deStage = JsonSerializer.Deserialize<IActivity>(text);
			if (deStage == null)
				throw new ArgumentNullException("Could not deserialize the activity stage!");
			return deStage;
		}

		private int FindNextStageIndex(ActScriptState state, WorkerResult executionResult)
		{
			var nextStageIndex = state.Stage + 1;
			if (executionResult.TargetActivity != "")
			{
				state.AppendToLog($"\tNext target activity is named {executionResult.TargetActivity}");
				nextStageIndex = state.Script.Stages.FindIndex(x => x.Name == executionResult.TargetActivity);
				if (nextStageIndex == -1)
					throw new Exception($"Could not find a activity named '{executionResult.TargetActivity}'");
				if (state.Script.Stages.Count(x => x.Name == executionResult.TargetActivity) > 1)
					state.AppendToLog($"Warning, multiple activities with the name '{executionResult.TargetActivity}' found! Using the first one...");
			}
			return nextStageIndex;
		}

		private void InsertResultIntoContextStore(ActScriptState state, string activityName, WorkerResult executionResult)
		{
			var values = executionResult.Context.GetContextValues();
			foreach (var valueKey in values.Keys)
				state.AddContext($"{activityName}.{valueKey}".ToLower(), values[valueKey]);
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

		private async Task<WorkerResult> ExecuteActivityAsync(IActivity act, ActScriptState state, CancellationToken token)
		{
			var targetKey = new ServiceKey(act.WorkerID, act.GetType().Name);
			if (_serviceCache.ContainsKey(targetKey))
			{
				var executor = _serviceCache[targetKey];
				return await ExecuteActivityAsync((dynamic)executor, (dynamic)act, state, token);
			}
			throw new Exception($"Unknown target activity executor '{targetKey}'! This probably means that the backend have not been set up to accept this type of action!");
		}

		private async Task<WorkerResult> ExecuteActivityAsync<T>(BaseWorker<T> worker, T act, ActScriptState state, CancellationToken token) where T : IActivity
		{
			return await worker.Execute(act, state, token);
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
