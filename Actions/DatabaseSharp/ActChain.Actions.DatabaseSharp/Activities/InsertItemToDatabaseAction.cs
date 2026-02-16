using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Activities
{
	public class InsertItemToDatabaseAction : IActivity
	{
		public string Name { get; set; } = "insertitemintodatabase";
		public string WorkerID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }

		public InsertItemToDatabaseAction(string name, string executorId, string targetSTP, Dictionary<string, string> arguments)
		{
			Name = name;
			WorkerID = executorId;
			TargetSTP = targetSTP;
			Arguments = arguments;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertItemToDatabaseAction(Name, WorkerID, TargetSTP, arguments);
		}
	}
}
