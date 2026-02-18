using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;

namespace ActFlow.Integrations.EMail.Activities
{
	public class SendEmailActivity : IActivity
	{
		public string Name { get; set; } = "sendanemail";
		public string WorkerID { get; set; } = "default";
		public IContext Answer { get; set; }

		public SendEmailActivity(string name, string workerId, IContext answer)
		{
			Name = name;
			WorkerID = workerId;
			Answer = answer;
		}

		public IActivity Clone() => new SendEmailActivity(Name, WorkerID, Answer.Clone());
	}
}
