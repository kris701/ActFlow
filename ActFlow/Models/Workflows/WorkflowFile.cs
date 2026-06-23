using ActFlow.Models.Contexts;

namespace ActFlow.Models.Workflows
{
	public class WorkflowFile
	{
		public string Path { get; set; }
		public WorkflowFileActionTypes Action { get; set; }
		public FileDirectories Directory { get; set; }
	}
}
