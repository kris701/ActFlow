using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class NoActivity : IActivity
	{
		public string Name { get; set; } = "noaction";
		public string WorkerID { get; set; } = "default";

		public NoActivity(string name, string workerId)
		{
			Name = name;
			WorkerID = workerId;
		}

		public IActivity Clone() => new NoActivity(Name, WorkerID);
	}
}
