using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.EMail.Activities
{
	public class SendEmailActivity : IActivity
	{
		public string Name { get; set; } = "sendanemail";
		public string WorkerID { get; set; } = "default";

		[Required]
		public IContext Answer { get; set; }
	}
}
