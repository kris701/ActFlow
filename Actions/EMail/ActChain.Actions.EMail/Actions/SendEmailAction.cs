using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class SendEmailAction : IAIAction
	{
		public string Name { get; set; } = "sendanemail";
		public string ExecutorID { get; set; } = "default";
		public IActionContext Answer { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public SendEmailAction(string name, string executorId, IActionContext answer)
		{
			Name = name;
			ExecutorID = executorId;
			Answer = answer;
		}

		public IAIAction Clone() => new SendEmailAction(Name, ExecutorID, Answer.Clone());
	}
}
