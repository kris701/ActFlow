using ActFlow.Models.Activities;

namespace ActFlow.Models.Workflows
{
	/// <summary>
	/// Definition of a workflow
	/// </summary>
	public class Workflow
	{
		/// <summary>
		/// Human readable name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// What the engine should do if something fails
		/// </summary>
		public WorkflowRetryBehaviour RetryBehaviour { get; set; } = WorkflowRetryBehaviour.None;
		/// <summary>
		/// A dictionary of global values to use across the workflow
		/// </summary>
		public Dictionary<string, string> Globals { get; set; } = new Dictionary<string, string>();
		/// <summary>
		/// The list of activities in the workflow
		/// </summary>
		public List<IActivity> Activities { get; set; } = new List<IActivity>();
	}
}
