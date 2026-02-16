using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class CreateContextAction : IActivity
	{
		public string Name { get; set; } = "createcontext";
		public string WorkerID { get; set; } = "default";
		public IContext Context { get; set; }

		public CreateContextAction(string name, string executorId, IContext context)
		{
			Name = name;
			WorkerID = executorId;
			Context = context;
		}

		public IActivity Clone() => new CreateContextAction(Name, WorkerID, Context);
	}
}
