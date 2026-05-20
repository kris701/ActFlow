namespace ActFlow.Models.Workflows
{
	/// <summary>
	/// Methods of retrying
	/// </summary>
	public enum WorkflowRetryBehaviour
	{
		/// <summary>
		/// Just crash
		/// </summary>
		None,
		/// <summary>
		/// Restart the entire workflow
		/// </summary>
		Workflow,
		/// <summary>
		/// Restart the failed activity
		/// </summary>
		Activity
	}
}
