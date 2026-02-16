using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class ReplyToEmailAction : IActivity
	{
		public string Name { get; set; } = "replytoemail";
		public string WorkerID { get; set; } = "default";
		public string ToMessageID { get; set; }
		public IContext Answer { get; set; }

		public ReplyToEmailAction(string name, string executorId, string toMessageID, IContext answer)
		{
			Name = name;
			WorkerID = executorId;
			ToMessageID = toMessageID;
			Answer = answer;
		}

		public IActivity Clone() => new ReplyToEmailAction(Name, WorkerID, ToMessageID, Answer.Clone());
	}
}
