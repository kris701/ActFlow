using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Time.Activities
{
	public class DelayActivity : IActivity
	{
		public string Name { get; set; } = "delay";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Delay { get; set; }

		public IActivity Clone() => new DelayActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			Delay = Delay
		};
	}
}
