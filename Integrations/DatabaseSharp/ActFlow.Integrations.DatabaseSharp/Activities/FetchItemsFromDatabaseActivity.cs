using ActFlow.Models.Activities;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class FetchItemsFromDatabaseActivity : IActivity
	{
		public string Name { get; set; } = "fetchitemsfromdatabases";
		public string WorkerID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
		public string TargetDeserializationType { get; set; }

		public FetchItemsFromDatabaseActivity(string name, string workerId, string targetSTP, Dictionary<string, string> arguments, string targetDeserializationType)
		{
			Name = name;
			WorkerID = workerId;
			TargetSTP = targetSTP;
			Arguments = arguments;
			TargetDeserializationType = targetDeserializationType;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new FetchItemsFromDatabaseActivity(Name, WorkerID, TargetSTP, arguments, TargetDeserializationType);
		}
	}
}
