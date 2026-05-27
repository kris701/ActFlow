using ActFlow.Models.Workflows;

namespace ActFlow.Archiver.Models
{
	public class ListWorkflowState
	{
		public Guid ID { get; set; }
		public string Name { get; set; }
		public WorkflowStatuses Status { get; set; }
		public DateTime? StartedAt { get; set; }
		public DateTime? EndedAt { get; set; }
	}
}
