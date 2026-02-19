using ActFlow.Models;
using ActFlow.Models.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ActFlow
{
	/// <summary>
	/// Main implementation for the ActFlow engine
	/// </summary>
	public class ActFlowEngine : IActFlowEngine
	{
		/// <summary>
		/// List of workers that the engine has to work with
		/// </summary>
		public List<IWorker> Workers { get; } = new List<IWorker>();
		/// <summary>
		/// List of currently active workflows
		/// </summary>
		public List<WorkflowState> ActiveWorkflows { get; } = new List<WorkflowState>();
		/// <summary>
		/// A limiter to how many activities a workflow can execute (to prevent infinite workflows)
		/// </summary>
		public int ActivityLimiter { get; set; } = 100;
		/// <summary>
		/// How many ms should pass before removing a completed workflow from the <seealso cref="ActiveWorkflows"/> list
		/// </summary>
		public int RemoveDelay { get; set; } = 60000;

		private readonly Dictionary<ServiceKey, IWorker> _serviceCache = new Dictionary<ServiceKey, IWorker>();
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);

		/// <summary>
		/// Main constructor with a set of workers
		/// </summary>
		/// <param name="workers"></param>
		public ActFlowEngine(List<IWorker> workers)
		{
			Workers = workers;
			foreach (var worker in workers)
			{
				var tpy = worker.GetType();
				var baseType = tpy.BaseType;
				var typeArg = baseType?.GenericTypeArguments[0].Name;
				var key = new ServiceKey(worker.ID, typeArg);
				if (!_serviceCache.ContainsKey(key))
					_serviceCache.Add(key, worker);
			}
		}

		#region Execute

		/// <summary>
		/// Execute a workflow and gets its state ID
		/// </summary>
		/// <param name="workflow"></param>
		/// <returns>ID of the workflow state</returns>
		public Guid Execute(Workflow workflow)
		{
			var newState = new WorkflowState(workflow);
			Task.Run(async () => await ExecuteAsync(newState));
			return newState.ID;
		}
		/// <summary>
		/// Execute a workflow state and gets its state ID
		/// </summary>
		/// <param name="state"></param>
		/// <returns>ID of the workflow state</returns>
		public Guid Execute(WorkflowState state)
		{
			Task.Run(async () => await ExecuteAsync(state));
			return state.ID;
		}

		/// <summary>
		/// Execute a workflow and wait for the result.
		/// </summary>
		/// <param name="workflow"></param>
		/// <returns></returns>
		public async Task<WorkflowState> ExecuteAsync(Workflow workflow)
		{
			var newState = new WorkflowState(workflow);
			return await ExecuteAsync(newState);
		}
		/// <summary>
		/// Execute a workflow state and wait for the result.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public async Task<WorkflowState> ExecuteAsync(WorkflowState state)
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
			while (state.ActivityIndex < state.Workflow.Activities.Count)
			{
				state.AppendToLog();
				state.AppendToLog($"\tStarting activity: {state.ActivityIndex}");
				if (state.Status == WorkflowStatuses.Canceled)
				{
					state.AppendToLog($"\tManual cancellation requested.");
					break;
				}

				try
				{
					var activity = state.Workflow.Activities[state.ActivityIndex].Clone();
					state.AppendToLog($"\tInitializing activity {activity.Name}");

					activity = ApplyContexts(state, activity);

					state.AppendToLog($"\tExecuting activity");
					await state.Update();
					var executionResult = await ExecuteActivityAsync(activity, state, state.TokenSource.Token);
					if (state.Status != WorkflowStatuses.Canceled)
						state.Status = WorkflowStatuses.Running;
					if (state.TokenSource.IsCancellationRequested)
						break;
					state.AppendToLog($"\tResulting activity context is a {executionResult.Context.GetType()}");
					state.AppendToLog($"\tResulting activity context content is {executionResult.Context.ToString()}");

					InsertResultIntoContextStore(state, activity.Name, executionResult);

					state.ActivityIndex = FindNextActivityIndex(state, executionResult);
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
					state.AppendToLogError($"Workflow exceded the activity limiter (of {ActivityLimiter} steps)! This most likely means something broke or you somehow managed to make the workflow loop.");
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

		private IActivity ApplyContexts(WorkflowState state, IActivity activity)
		{
			var text = JsonSerializer.Serialize(activity);
			var matches = _variableRegex.Matches(text);
			foreach (Match match in matches)
			{
				var key = match.Groups[1].Value.ToLower();
				if (state.ContextStore.Keys.Contains(key))
					text = text.Replace(match.Groups[0].Value, state.ContextStore[key]);
				else
					throw new Exception($"Variable '{key}' was not found in the current state!");
			}
			var deActivity = JsonSerializer.Deserialize<IActivity>(text);
			if (deActivity == null)
				throw new ArgumentNullException("Could not deserialize the activity!");
			return deActivity;
		}

		private int FindNextActivityIndex(WorkflowState state, WorkerResult executionResult)
		{
			var nextActivityIndex = state.ActivityIndex + 1;
			if (executionResult.TargetActivity != "")
			{
				state.AppendToLog($"\tNext target activity is named {executionResult.TargetActivity}");
				nextActivityIndex = state.Workflow.Activities.FindIndex(x => x.Name == executionResult.TargetActivity);
				if (nextActivityIndex == -1)
					throw new Exception($"Could not find a activity named '{executionResult.TargetActivity}'");
				if (state.Workflow.Activities.Count(x => x.Name == executionResult.TargetActivity) > 1)
					state.AppendToLog($"Warning, multiple activities with the name '{executionResult.TargetActivity}' found! Using the first one...");
			}
			return nextActivityIndex;
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				Task.Run(async () =>
				{
					await Task.Delay(RemoveDelay);
					ActiveWorkflows.Remove(state);
				});
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
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

		#endregion

		#region Cancelation

		/// <summary>
		/// Request a cancel of a given workflow state id
		/// </summary>
		/// <param name="id"></param>
		public void Cancel(Guid id)
		{
			Task.Run(async () => CancelAsync(id));
		}
		/// <summary>
		/// Cancel a given workflow by its id and wait for it to finish
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task CancelAsync(Guid id)
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

		/// <summary>
		/// Request all workflows to cancel
		/// </summary>
		public void CancelAll()
		{
			Task.Run(async () => CancelAllAsync());
		}
		/// <summary>
		/// Cancel all workflows and wait for them all to stop.
		/// </summary>
		/// <returns></returns>
		public async Task CancelAllAsync()
		{
			foreach (var item in ActiveWorkflows)
				await CancelAsync(item.ID);
		}

		#endregion

		/// <summary>
		/// Updates a currently running workflow state with a new workflow definition.
		/// This is only possible if the workflow is at an activity that implements <seealso cref="Models.Activities.IUpdatableWorkflowActivity"/>
		/// and that the workflow state is <seealso cref="ActFlow.Models.Workflows.WorkflowStatuses.AwaitingUpdate"/>
		/// </summary>
		/// <param name="stateId"></param>
		/// <param name="workflow"></param>
		/// <returns></returns>
		public async Task<WorkflowState> UpdateWorkflowAsync(Guid stateId, Workflow workflow)
		{
			var state = ActiveWorkflows.FirstOrDefault(x => x.ID == stateId);
			if (state != null)
			{
				if (state.Workflow.Activities[state.ActivityIndex] is not IUpdatableWorkflowActivity)
					throw new Exception($"Can only update the workflow on activities that implements '{nameof(IUpdatableWorkflowActivity)}'");
				if (state.Status != WorkflowStatuses.AwaitingUpdate)
					throw new Exception("Current state is not awaiting any updates!");

				state.AppendToLog("Applying user input");
				var combinedState = new WorkflowState(state);
				combinedState.Workflow = workflow;
				state.IsProcessingUserInput = true;
				await CancelAsync(state.ID);
				combinedState.Status = WorkflowStatuses.NotStarted;
				combinedState.AppendToLog("Rerunning workflow...");
				await combinedState.Update();
				Execute(combinedState);
				return combinedState;
			}
			throw new Exception($"No workflow log item with the id '{stateId}' found!");
		}
	}
}
