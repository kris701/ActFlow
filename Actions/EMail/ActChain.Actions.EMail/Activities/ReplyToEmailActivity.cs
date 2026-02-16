using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class ReplyToEmailActivity : IActivity
	{
		public string Name { get; set; } = "replytoemail";
		public string WorkerID { get; set; } = "default";
		public string ToMessageID { get; set; }
		public IContext Answer { get; set; }

		public ReplyToEmailActivity(string name, string workerId, string toMessageID, IContext answer)
		{
			Name = name;
			WorkerID = workerId;
			ToMessageID = toMessageID;
			Answer = answer;
		}

		public IActivity Clone() => new ReplyToEmailActivity(Name, WorkerID, ToMessageID, Answer.Clone());
	}
}
