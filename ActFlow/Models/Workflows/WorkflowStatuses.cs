namespace ActFlow.Models.Workflows
{
	/// <summary>
	/// The different states a workflow can be in
	/// </summary>
	public enum WorkflowStatuses
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Not started yet
		/// </summary>
		NotStarted,
		/// <summary>
		/// The workflow is actively running
		/// </summary>
		Running,
		/// <summary>
		/// Workflow has failed and is no longer running
		/// </summary>
		Failed,
		/// <summary>
		/// Workflow has ended in success
		/// </summary>
		Succeeded,
		/// <summary>
		/// Workflow was cancled
		/// </summary>
		Canceled,
		/// <summary>
		/// Workflow is awaiting manual input
		/// </summary>
		AwaitingUpdate
	}
}
