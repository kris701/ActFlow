using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Activities
{
	public class FetchItemsFromDatabaseAction : IActivity
	{
		public string Name { get; set; } = "fetchitemsfromdatabases";
		public string WorkerID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
		public string TargetDeserializationType { get; set; }

		public FetchItemsFromDatabaseAction(string name, string executorId, string targetSTP, Dictionary<string, string> arguments, string targetDeserializationType)
		{
			Name = name;
			WorkerID = executorId;
			TargetSTP = targetSTP;
			Arguments = arguments;
			TargetDeserializationType = targetDeserializationType;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new FetchItemsFromDatabaseAction(Name, WorkerID, TargetSTP, arguments, TargetDeserializationType);
		}
	}
}
