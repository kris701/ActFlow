using ActFlow.Models.Activities;

namespace ActFlow.Models.Workflows
{
	public interface IWorkflow
	{
		public string Name { get; set; }
		public Dictionary<string, string> Globals { get; set; }
		public List<IActivity> Activities { get; set; }
	}
}
