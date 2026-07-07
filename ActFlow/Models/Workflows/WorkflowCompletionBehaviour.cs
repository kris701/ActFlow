namespace ActFlow.Models.Workflows
{
	/// <summary>
	/// What to do at the end of a workflow run
	/// </summary>
	public enum WorkflowCompletionBehaviour
	{
		/// <summary>
		/// Nothing special
		/// </summary>
		None,
		/// <summary>
		/// Puts a fresh version of the workflow in the queue again
		/// </summary>
		ReQueue
	}
}
