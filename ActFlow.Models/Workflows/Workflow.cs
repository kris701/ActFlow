using ActChain.Models.Activities;
using ActChain.Models.Workflows;

namespace ActChain.Models.Scripts
{
	public class Workflow : IWorkflow
	{
		public string Name { get; set; }
		public Dictionary<string, string> Globals { get; set; } = new Dictionary<string, string>();
		public List<IActivity> Stages { get; set; } = new List<IActivity>();
	}
}
