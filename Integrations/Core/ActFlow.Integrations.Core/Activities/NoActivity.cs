using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Core.Activities
{
	public class NoActivity : IActivity
	{
		public string Name { get; set; } = "noaction";
		public string WorkerID { get; set; } = "default";
	}
}
