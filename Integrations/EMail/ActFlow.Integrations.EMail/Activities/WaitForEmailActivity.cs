using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.EMail.Activities
{
	public class WaitForEmailActivity : IActivity, IUpdatableWorkflowActivity
	{
		public string Name { get; set; } = "waitforemail";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string SenderEmail { get; set; }
		[Required]
		public string RecieverEmail { get; set; }
		[Required]
		public string ConversationID { get; set; }

		public IActivity Clone() => new WaitForEmailActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			SenderEmail = SenderEmail,
			RecieverEmail = RecieverEmail,
			ConversationID = ConversationID
		};
	}
}
