using ActFlow.Archiver.Models;

namespace ActFlow.Archiver
{
	/// <summary>
	/// Interface to handle archival of completed workflows
	/// </summary>
	public interface IWorkflowArchive
	{
		/// <summary>
		/// The path to where to save completed workflow runs
		/// </summary>
		public string CompletedDirectory { get; set; }

		/// <summary>
		/// Initialize the archiver
		/// </summary>
		/// <returns></returns>
		public Task Initialize();

		/// <summary>
		/// Get a simplified list of all completed workflows
		/// </summary>
		/// <returns></returns>
		public List<ListWorkflowState> GetAllCompletedWorkflows();
		/// <summary>
		/// Gets details information on a single workflow
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CompletedWorkflowState? GetCompletedWorkflow(Guid id);
		/// <summary>
		/// Removed a completed workflow
		/// </summary>
		/// <param name="id"></param>
		public void RemoveCompletedWorkflow(Guid id);
	}
}
