using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class NoAction : IActivity
	{
		public string Name { get; set; } = "noaction";
		public string WorkerID { get; set; } = "default";

		public NoAction(string name, string executorId)
		{
			Name = name;
			WorkerID = executorId;
		}

		public IActivity Clone() => new NoAction(Name, WorkerID);
	}
}
