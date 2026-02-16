using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.EMail.Actions
{
	public class SendEmailAction : IActivity
	{
		public string Name { get; set; } = "sendanemail";
		public string WorkerID { get; set; } = "default";
		public IContext Answer { get; set; }

		public SendEmailAction(string name, string executorId, IContext answer)
		{
			Name = name;
			WorkerID = executorId;
			Answer = answer;
		}

		public IActivity Clone() => new SendEmailAction(Name, WorkerID, Answer.Clone());
	}
}
