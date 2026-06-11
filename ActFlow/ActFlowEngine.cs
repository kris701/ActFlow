using ActFlow.Helpers;
using ActFlow.Models;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ToolsSharp;

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
		/// The path to the persistent data folder
		/// </summary>
		public string PersistentDirectory { get; set; } = ".persistent";

		/// <summary>
		/// The path to the temporary data folder
		/// </summary>
		public string TemporaryDirectory { get; set; } = ".runners";

		/// <summary>
		/// The path to where to save completed workflow runs
		/// If this is set to null, the workflows will simply be discarded on completion.
		/// </summary>
		public string? CompletedDirectory { get; set; } = ".completed";

		private readonly Dictionary<ServiceKey, IWorker> _serviceCache = new Dictionary<ServiceKey, IWorker>();
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);

		/// <summary>
		/// Main constructor with a set of workers
		/// </summary>
		/// <param name="workers"></param>
		public ActFlowEngine(List<IWorker> workers)
		{
			Workers = workers;

			if (!Constants.SerializerOpts.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)) &&
				!Constants.SerializerOpts.Converters.IsReadOnly)
				Constants.SerializerOpts.Converters.Add(new JsonStringEnumConverter());
		}

		/// <summary>
		/// Initialize all the needed directories and workers
		/// </summary>
		/// <returns></returns>
		public async Task Initialize()
		{
			foreach (var worker in Workers)
			{
				worker.PersistenDirectory = PersistentDirectory;

				var tpy = worker.GetType();
				var baseType = tpy.BaseType;
				var typeArg = baseType?.GenericTypeArguments[0].Name;
				var key = new ServiceKey(worker.ID, typeArg);
				if (!_serviceCache.ContainsKey(key))
					_serviceCache.Add(key, worker);
			}

			DirectoryHelper.DeleteDirectory(TemporaryDirectory);
			if (!Directory.Exists(TemporaryDirectory))
				Directory.CreateDirectory(TemporaryDirectory);

			if (!Directory.Exists(PersistentDirectory))
				Directory.CreateDirectory(PersistentDirectory);

			if (CompletedDirectory != null && !Directory.Exists(CompletedDirectory))
				Directory.CreateDirectory(CompletedDirectory);
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

			var tmpDirectory = Path.Combine(TemporaryDirectory, state.ID.ToString());
			if (Directory.Exists(tmpDirectory))
				Directory.Delete(tmpDirectory);
			Directory.CreateDirectory(tmpDirectory);

			state.AppendToLog("Temporary folder created");

			if (state.ActivityIndex < 0)
				state.ActivityIndex = 0;

			int activityCount = state.ActivityIndex;
			state.Status = WorkflowStatuses.Running;
			state.StartedAt = DateTime.UtcNow;
			await state.Update();
			while (state.ActivityIndex < state.Workflow.Activities.Count)
			{
				state.AppendToLog($"Starting activity: {state.ActivityIndex}");
				if (state.Status == WorkflowStatuses.Canceled)
				{
					state.AppendToLog($"Manual cancellation requested.");
					break;
				}

				try
				{
					await ExecuteActivityAsync(state, tmpDirectory);
				}
				catch (Exception ex)
				{
					if (state.TokenSource.IsCancellationRequested)
					{
						state.Status = WorkflowStatuses.Canceled;
						break;
					}

					state.AppendToLog(WorkflowLogTypes.Error, $"Message: {ex.Message}");
					state.AppendToLog(WorkflowLogTypes.Error, $"Trace: {ex.StackTrace}");

					if (state.Workflow.RetryBehaviour == WorkflowRetryBehaviour.None)
					{
						state.Status = WorkflowStatuses.Failed;
						break;
					}
					else if (state.Workflow.RetryBehaviour == WorkflowRetryBehaviour.Activity)
					{
						state.AppendToLog(WorkflowLogTypes.Warn, $"Retrying activity in 10 seconds...");
						await Task.Delay(TimeSpan.FromSeconds(10));
						continue;
					}
					else if (state.Workflow.RetryBehaviour == WorkflowRetryBehaviour.Workflow)
					{
						state.AppendToLog(WorkflowLogTypes.Warn, $"Retrying entire workflow in 10 seconds...");
						await Task.Delay(TimeSpan.FromSeconds(10));
						ActiveWorkflows.Remove(state);
						return await ExecuteAsync(state.Workflow);
					}
				}

				if (state.TokenSource.IsCancellationRequested)
					break;

				activityCount++;
				if (activityCount > ActivityLimiter)
				{
					state.AppendToLog(WorkflowLogTypes.Warn, $"Workflow exceded the activity limiter (of {ActivityLimiter} steps)! This most likely means something broke or you somehow managed to make the workflow loop.");
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

		private async Task ProcessCompleted(WorkflowState state)
		{
			if (!state.IsProcessingUserInput)
				await state.Complete();

			if (CompletedDirectory != null)
			{
				state.AppendToLog($"Moving workflow state to completed folder...");
				await state.Update();
				var path = Path.Combine(CompletedDirectory, state.ID.ToString());
				if (Directory.Exists(path))
					DirectoryHelper.DeleteDirectory(path);
				Directory.CreateDirectory(path);

				var workflowFile = Path.Combine(path, "state.json");
				await File.WriteAllTextAsync(workflowFile, JsonSerializer.Serialize(state, Constants.SerializerOpts));

				var orgTmpDirectory = Path.Combine(TemporaryDirectory, state.ID.ToString());
				var newTmpDirectory = Path.Combine(path, "tmp");
				if (Directory.Exists(newTmpDirectory))
					Directory.CreateDirectory(newTmpDirectory);
				if (Directory.Exists(orgTmpDirectory))
					DirectoryHelper.CopyFilesRecursively(orgTmpDirectory, newTmpDirectory);
			}

			ActiveWorkflows.Remove(state);
			var tmpDirectory = Path.Combine(TemporaryDirectory, state.ID.ToString());
			if (Directory.Exists(tmpDirectory))
				Directory.Delete(tmpDirectory);
		}

		private async Task ExecuteActivityAsync(WorkflowState state, string tmpDirectory)
		{
			var activity = state.Workflow.Activities[state.ActivityIndex];
			state.AppendToLog($"Initializing activity {activity.Name}");

			activity = ActivityHelpers.ApplyContexts(state, activity);

			state.AppendToLog($"Executing activity");
			await state.Update();
			var executionResult = await ExecuteActivityAsync(activity, state, state.TokenSource.Token, tmpDirectory);
			if (state.Status != WorkflowStatuses.Canceled)
				state.Status = WorkflowStatuses.Running;
			if (state.TokenSource.IsCancellationRequested)
				return;
			state.AppendToLog($"Resulting activity context is a {executionResult.Context.GetType()}");
			var contextString = executionResult.Context.ToString();
			if (contextString != null)
				contextString = contextString.Length > 100 ? contextString.Substring(0, 100) : contextString;
			state.AppendToLog($"Resulting activity context content is {contextString}");

			WorkflowStateHelpers.InsertResultIntoContextStore(state, activity.Name, executionResult);

			state.ActivityIndex = WorkflowStateHelpers.FindNextActivityIndex(state, executionResult);
		}

		private async Task<WorkerResult> ExecuteActivityAsync(IActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var targetKey = new ServiceKey(act.WorkerID, act.GetType().Name);
			if (_serviceCache.ContainsKey(targetKey))
			{
				var executor = _serviceCache[targetKey];
				return await ActivityHelpers.ExecuteActivityAsync((dynamic)executor, (dynamic)act, state, token, tmpDirectory);
			}
			throw new Exception($"Unknown target activity executor '{targetKey}'! This probably means that the backend have not been set up to accept this type of action!");
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
				target.AppendToLog(WorkflowLogTypes.Warn, "Cancellation requested");
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
		/// This is only possible if the workflow is at an activity that implements <seealso cref="Models.Activities.IHumanInput"/>
		/// and that the workflow state is <seealso cref="ActFlow.Models.Workflows.WorkflowStatuses.AwaitingHumanInput"/>
		/// </summary>
		/// <param name="stateId"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public void ApplyHumanInput(Guid stateId, IContext input)
		{
			var state = ActiveWorkflows.FirstOrDefault(x => x.ID == stateId);
			if (state != null)
			{
				var currentAct = state.Workflow.Activities[state.ActivityIndex];
				if (currentAct is not IHumanInput humanAct)
					throw new Exception($"Can only update the workflow on activities that implements '{nameof(IHumanInput)}'");
				if (state.Status != WorkflowStatuses.AwaitingHumanInput)
					throw new Exception("Current state is not awaiting any updates!");

				state.AppendToLog(WorkflowLogTypes.Warn, "Applying human input");
				humanAct.Apply(input);
			}
			else
				throw new Exception($"No workflow log item with the id '{stateId}' found!");
		}
	}
}
