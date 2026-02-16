using ActChain.Models.Activities;
using ActChain.Models.Contexts;

namespace ActChain.Integrations.Core.Activities
{
	public class CreateContextActivity : IActivity
	{
		public string Name { get; set; } = "createcontext";
		public string WorkerID { get; set; } = "default";
		public IContext Context { get; set; }

		public CreateContextActivity(string name, string workerId, IContext context)
		{
			Name = name;
			WorkerID = workerId;
			Context = context;
		}

		public IActivity Clone() => new CreateContextActivity(Name, WorkerID, Context);
	}
}
