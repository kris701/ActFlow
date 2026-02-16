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
		public List<WorkflowState> ActiveWorkflows { get; } = new List<WorkflowState>();
		public int ActivityLimiter { get; set; } = 100;
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

		public async Task<WorkflowState> Execute(Workflow workflow)
		{
			var newState = new WorkflowState(workflow);
			return await Execute(newState);
		}

		public async Task<WorkflowState> Execute(WorkflowState state)
		{
			ActiveWorkflows.Add(state);
			if (state.ActivityIndex == 0)
				state.AppendToLog("Workflow started!");
			else
				state.AppendToLog("Workflow continued...");

			if (state.ActivityIndex < 0)
				state.ActivityIndex = 0;

			int activityCount = state.ActivityIndex;
			state.Status = WorkflowStatuses.Running;
			state.StartedAt = DateTime.UtcNow;
			await state.Update();
			while (state.ActivityIndex < state.Workflow.Stages.Count)
			{
				state.AppendToLog();
				state.AppendToLog($"\tStarting stage: {state.ActivityIndex}");
				if (state.Status == WorkflowStatuses.Canceled)
				{
					state.AppendToLog($"\tManual cancellation requested.");
					break;
				}

				try
				{
					var stage = state.Workflow.Stages[state.ActivityIndex].Clone();
					state.AppendToLog($"\tInitializing activity {stage.Name}");

					stage = ApplyContexts(state, stage);

					state.AppendToLog($"\tExecuting activity");
					if (state.Status != WorkflowStatuses.Canceled && stage is IAwaitInputActivity)
						state.Status = WorkflowStatuses.AwaitingInput;
					await state.Update();
					var executionResult = await ExecuteActivityAsync(stage, state, state.TokenSource.Token);
					if (state.Status != WorkflowStatuses.Canceled)
						state.Status = WorkflowStatuses.Running;
					if (state.TokenSource.IsCancellationRequested)
						break;
					state.AppendToLog($"\tResulting activity context is a {executionResult.Context.GetType()}");
					state.AppendToLog($"\tResulting activity context content is {executionResult.Context.ToString()}");

					InsertResultIntoContextStore(state, stage.Name, executionResult);

					state.ActivityIndex = FindNextStageIndex(state, executionResult);
				}
				catch (Exception ex)
				{
					state.AppendToLogError($"Message: {ex.Message}");
					state.AppendToLogError($"Trace: {ex.StackTrace}");
					state.Status = WorkflowStatuses.Failed;
					break;
				}

				activityCount++;
				if (activityCount > ActivityLimiter)
				{
					state.AppendToLogError($"Workflow exceded the stage limiter (of {ActivityLimiter} steps)! This most likely means something broke or you somehow managed to make the Chain loop.");
					state.Status = WorkflowStatuses.Canceled;
					break;
				}
			}
			if (state.Status == WorkflowStatuses.Running)
				state.Status = WorkflowStatuses.Succeeded;
			state.EndedAt = DateTime.UtcNow;
			state.AppendToLog($"Workflow ended!");

			await state.Update();

			await ProcessCompleted(state);
			return state;
		}

		private IActivity ApplyContexts(WorkflowState state, IActivity stage)
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

		private int FindNextStageIndex(WorkflowState state, WorkerResult executionResult)
		{
			var nextStageIndex = state.ActivityIndex + 1;
			if (executionResult.TargetActivity != "")
			{
				state.AppendToLog($"\tNext target activity is named {executionResult.TargetActivity}");
				nextStageIndex = state.Workflow.Stages.FindIndex(x => x.Name == executionResult.TargetActivity);
				if (nextStageIndex == -1)
					throw new Exception($"Could not find a activity named '{executionResult.TargetActivity}'");
				if (state.Workflow.Stages.Count(x => x.Name == executionResult.TargetActivity) > 1)
					state.AppendToLog($"Warning, multiple activities with the name '{executionResult.TargetActivity}' found! Using the first one...");
			}
			return nextStageIndex;
		}

		private void InsertResultIntoContextStore(WorkflowState state, string activityName, WorkerResult executionResult)
		{
			var values = executionResult.Context.GetContextValues();
			foreach (var valueKey in values.Keys)
				state.AddContext($"{activityName}.{valueKey}".ToLower(), values[valueKey]);
		}

		private async Task ProcessCompleted(WorkflowState state)
		{
			if (!state.IsProcessingUserInput)
				await state.Complete();

			if (state.TokenSource.IsCancellationRequested)
				ActiveWorkflows.Remove(state);
			else
			{
				state.AppendToLog($"Removing from Active List in {RemoveDelay}ms");
				await state.Update();
				Task.Run(async () =>
				{
					await Task.Delay(RemoveDelay);
					ActiveWorkflows.Remove(state);
				});
			}
		}

		public async Task Cancel(Guid id)
		{
			var target = ActiveWorkflows.FirstOrDefault(x => x.ID == id);
			if (target != null)
			{
				target.AppendToLog("Cancellation requested");
				target.TokenSource.Cancel();
				while (ActiveWorkflows.Contains(target))
					await Task.Delay(500);
				target.Status = WorkflowStatuses.Canceled;
				await target.Update();
			}
		}

		public async Task CancelAll()
		{
			foreach (var item in ActiveWorkflows)
				await Cancel(item.ID);
		}

		private async Task<WorkerResult> ExecuteActivityAsync(IActivity act, WorkflowState state, CancellationToken token)
		{
			var targetKey = new ServiceKey(act.WorkerID, act.GetType().Name);
			if (_serviceCache.ContainsKey(targetKey))
			{
				var executor = _serviceCache[targetKey];
				return await ExecuteActivityAsync((dynamic)executor, (dynamic)act, state, token);
			}
			throw new Exception($"Unknown target activity executor '{targetKey}'! This probably means that the backend have not been set up to accept this type of action!");
		}

		private async Task<WorkerResult> ExecuteActivityAsync<T>(BaseWorker<T> worker, T act, WorkflowState state, CancellationToken token) where T : IActivity
		{
			return await worker.Execute(act, state, token);
		}

		public async Task<WorkflowState> UserInput(Guid stateId, WorkflowState workflow)
		{
			var state = ActiveWorkflows.FirstOrDefault(x => x.ID == stateId);
			if (state != null)
			{
				state.AppendToLog("Applying user input");
				var combinedState = new WorkflowState(state);
				combinedState.Workflow = workflow.Workflow;
				state.IsProcessingUserInput = true;
				await Cancel(state.ID);
				combinedState.Status = WorkflowStatuses.NotStarted;
				combinedState.AppendToLog("Rerunning chain...");
				await combinedState.Update();
				Execute(combinedState);
				return combinedState;
			}
			throw new Exception($"No chain log item with the id '{stateId}' found!");
		}
	}
}
