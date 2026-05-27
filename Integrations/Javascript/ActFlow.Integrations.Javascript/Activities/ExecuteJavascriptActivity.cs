using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Javascript.Activities
{
	public class ExecuteJavascriptActivity : IActivity
	{
		public string WorkerID { get; set; } = "default";
		public string Name { get; set; } = "executejs";

		[Required]
		public string Code { get; set; }
	}
}
