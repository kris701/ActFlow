using ActFlow.Models.Activities;

namespace ActFlow.Models.Workflows
{
	public class Workflow
	{
		public string Name { get; set; }
		public WorkflowRetryBehaviour RetryBehaviour { get; set; } = WorkflowRetryBehaviour.None;
		public Dictionary<string, string> Globals { get; set; } = new Dictionary<string, string>();
		public List<IActivity> Activities { get; set; } = new List<IActivity>();
	}
}
