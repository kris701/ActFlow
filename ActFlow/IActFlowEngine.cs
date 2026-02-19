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
		/// How many ms should pass before removing a completed workflow from the <seealso cref="ActiveWorkflows"/> list
		/// </summary>
		public int RemoveDelay { get; set; }

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
		/// Updates a currently running workflow state with a new workflow definition.
		/// This is only possible if the workflow is at an activity that implements <seealso cref="Models.Activities.IUpdatableWorkflowActivity"/>
		/// and that the workflow state is <seealso cref="ActFlow.Models.Workflows.WorkflowStatuses.AwaitingUpdate"/>
		/// </summary>
		/// <param name="stateId"></param>
		/// <param name="workflow"></param>
		/// <returns></returns>
		public Task<WorkflowState> UpdateWorkflowAsync(Guid stateId, Workflow workflow);
	}
}
