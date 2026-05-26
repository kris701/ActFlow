using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.EMail.Activities
{
	public class ReplyToEmailActivity : IActivity
	{
		public string Name { get; set; } = "replytoemail";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string ToMessageID { get; set; }
		[Required]
		public IContext Answer { get; set; }

		public IActivity Clone() => new ReplyToEmailActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			ToMessageID = ToMessageID,
			Answer = Answer.Clone()
		};
	}
}
