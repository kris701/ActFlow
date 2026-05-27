using ActFlow.Models.Workflows;

namespace ActFlow.Archiver.Models
{
	public class CompletedWorkflowState
	{
		public WorkflowState State { get; set; }
		public List<string> Files { get; set; }
	}
}
