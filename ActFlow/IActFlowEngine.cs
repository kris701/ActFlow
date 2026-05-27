using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow
{
	/// <summary>
	/// Main interface for the ActFlow engine
	/// </summary>
	public interface IActFlowEngine
	{
		/// <summary>
		/// List of workers that the engine has to work with
		/// </summary>
		public List<IWorker> Workers { get; }
		/// <summary>
		/// List of currently active workflows
		/// </summary>
		public List<WorkflowState> ActiveWorkflows { get; }
		/// <summary>
		/// A limiter to how many activities a workflow can execute (to prevent infinite workflows)
		/// </summary>
		public int ActivityLimiter { get; set; }

		/// <summary>
		/// The path to the persistent data folder
		/// </summary>
		public string PersistentDirectory { get; set; }

		/// <summary>
		/// The path to the temporary data folder
		/// </summary>
		public string TemporaryDirectory { get; set; }

		/// <summary>
		/// The path to where to save completed workflow runs.
		/// If this is set to null, the workflows will simply be discarded on completion.
		/// </summary>
		public string? CompletedDirectory { get; set; }

		/// <summary>
		/// Initialize all the needed directories and workers
		/// </summary>
		/// <returns></returns>
		public Task Initialize();

		/// <summary>
		/// Execute a workflow and gets its state ID
		/// </summary>
		/// <param name="workflow"></param>
		/// <returns>ID of the workflow state</returns>
		public Guid Execute(Workflow workflow);
		/// <summary>
		/// Execute a workflow state and gets its state ID
		/// </summary>
		/// <param name="state"></param>
		/// <returns>ID of the workflow state</returns>
		public Guid Execute(WorkflowState state);

		/// <summary>
		/// Execute a workflow and wait for the result.
		/// </summary>
		/// <param name="workflow"></param>
		/// <returns></returns>
		public Task<WorkflowState> ExecuteAsync(Workflow workflow);
		/// <summary>
		/// Execute a workflow state and wait for the result.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public Task<WorkflowState> ExecuteAsync(WorkflowState state);

		/// <summary>
		/// Request a cancel of a given workflow state id
		/// </summary>
		/// <param name="id"></param>
		public void Cancel(Guid id);
		/// <summary>
		/// Cancel a given workflow by its id and wait for it to finish
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Task CancelAsync(Guid id);

		/// <summary>
		/// Request all workflows to cancel
		/// </summary>
		public void CancelAll();
		/// <summary>
		/// Cancel all workflows and wait for them all to stop.
		/// </summary>
		/// <returns></returns>
		public Task CancelAllAsync();

		/// <summary>
		/// Apply human input to a given workflow.
		/// This is only possible if the workflow is at an activity that implements <seealso cref="Models.Activities.IHumanInput"/>
		/// and that the workflow state is <seealso cref="ActFlow.Models.Workflows.WorkflowStatuses.AwaitingHumanInput"/>
		/// </summary>
		/// <param name="stateId"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public void ApplyHumanInput(Guid stateId, IContext input);
	}
}
