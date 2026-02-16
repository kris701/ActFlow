using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class InsertGlobalsActivity : IActivity
	{
		public string Name { get; set; } = "insertglobal";
		public string WorkerID { get; set; } = "default";
		public Dictionary<string, string> Arguments { get; set; }

		public InsertGlobalsActivity(string name, string workerId, Dictionary<string, string> arguments)
		{
			Name = name;
			WorkerID = workerId;
			Arguments = arguments;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertGlobalsActivity(Name, WorkerID, arguments);
		}
	}
}
