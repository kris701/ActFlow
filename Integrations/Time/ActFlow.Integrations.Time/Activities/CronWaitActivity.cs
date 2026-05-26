using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Time.Activities
{
	public class CronWaitActivity : IActivity
	{
		public string Name { get; set; } = "cronwait";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string CronExpression { get; set; }

		public IActivity Clone() => new CronWaitActivity() {
			Name = Name,
			WorkerID = WorkerID,
			CronExpression = CronExpression
		};
	}
}
