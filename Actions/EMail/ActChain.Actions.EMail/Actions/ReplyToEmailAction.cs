using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class ReplyToEmailAction : IAIAction
	{
		public string Name { get; set; } = "replytoemail";
		public string ExecutorID { get; set; } = "default";
		public string ToMessageID { get; set; }
		public IActionContext Answer { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ReplyToEmailAction(string name, string executorId, string toMessageID, IActionContext answer)
		{
			Name = name;
			ExecutorID = executorId;
			ToMessageID = toMessageID;
			Answer = answer;
		}

		public IAIAction Clone() => new ReplyToEmailAction(Name, ExecutorID, ToMessageID, Answer.Clone());
	}
}
