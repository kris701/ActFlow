using ActFlow.Models.Workflows;

namespace ActFlow.Archiver.Models
{
	internal class SimpleWorkflowState
	{
		public Guid ID { get; set; }
		public WorkflowStatuses Status { get; set; }
		public DateTime StartedAt { get; set; }
		public DateTime EndedAt { get; set; }
		public SimpleWorkflow Workflow { get; set; }
	}
}
