using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class WaitForEmailAction : IActivity, IAwaitInputActivity
	{
		public string Name { get; set; } = "waitforemail";
		public string WorkerID { get; set; } = "default";
		public string SenderEmail { get; set; }
		public string RecieverEmail { get; set; }
		public string ConversationID { get; set; }

		public WaitForEmailAction(string name, string executorId, string senderEmail, string recieverEmail, string conversationID)
		{
			Name = name;
			WorkerID = executorId;
			SenderEmail = senderEmail;
			RecieverEmail = recieverEmail;
			ConversationID = conversationID;
		}

		public IActivity Clone() => new WaitForEmailAction(Name, WorkerID, SenderEmail, RecieverEmail, ConversationID);
	}
}
