using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class WaitForEmailActivity : IActivity, IAwaitInputActivity
	{
		public string Name { get; set; } = "waitforemail";
		public string WorkerID { get; set; } = "default";
		public string SenderEmail { get; set; }
		public string RecieverEmail { get; set; }
		public string ConversationID { get; set; }

		public WaitForEmailActivity(string name, string workerId, string senderEmail, string recieverEmail, string conversationID)
		{
			Name = name;
			WorkerID = workerId;
			SenderEmail = senderEmail;
			RecieverEmail = recieverEmail;
			ConversationID = conversationID;
		}

		public IActivity Clone() => new WaitForEmailActivity(Name, WorkerID, SenderEmail, RecieverEmail, ConversationID);
	}
}
