using ActFlow.Models.Activities;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class FetchItemsFromDatabaseActivity : IActivity
	{
		public string Name { get; set; } = "fetchitemsfromdatabases";
		public string WorkerID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
		public int ResultTable { get; set; } = 0;
		public Dictionary<string, string> ResultMap { get; set; }

		public FetchItemsFromDatabaseActivity(string name, string workerId, string targetSTP, Dictionary<string, string> arguments, int resultTable, Dictionary<string, string> resultMap)
		{
			Name = name;
			WorkerID = workerId;
			TargetSTP = targetSTP;
			Arguments = arguments;
			ResultTable = resultTable;
			ResultMap = resultMap;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			var resultMap = new Dictionary<string, string>();
			foreach (var key in ResultMap.Keys)
				resultMap.Add(key, ResultMap[key]);
			return new FetchItemsFromDatabaseActivity(Name, WorkerID, TargetSTP, arguments, ResultTable, resultMap);
		}
	}
}
