using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Activities
{
	public class InsertChainFromDatabaseAction : IActivity
	{
		public string Name { get; set; } = "insertchain";
		public string WorkerID { get; set; } = "default";

		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }

		public InsertChainFromDatabaseAction(string name, string executorID, string targetSTP, Dictionary<string, string> arguments)
		{
			Name = name;
			WorkerID = executorID;
			TargetSTP = targetSTP;
			Arguments = arguments;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertChainFromDatabaseAction(Name, WorkerID, TargetSTP, arguments);
		}
	}
}
