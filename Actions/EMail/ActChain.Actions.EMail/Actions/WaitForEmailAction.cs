using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class WaitForEmailAction : IAIAction, IAwaitInputAction
	{
		public string Name { get; set; } = "waitforemail";
		public string ExecutorID { get; set; } = "default";
		public string SenderEmail { get; set; }
		public string RecieverEmail { get; set; }
		public string ConversationID { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public WaitForEmailAction(string name, string executorId, string senderEmail, string recieverEmail, string conversationID)
		{
			Name = name;
			ExecutorID = executorId;
			SenderEmail = senderEmail;
			RecieverEmail = recieverEmail;
			ConversationID = conversationID;
		}

		public IAIAction Clone() => new WaitForEmailAction(Name, ExecutorID, SenderEmail, RecieverEmail, ConversationID);
	}
}
