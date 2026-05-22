using ActFlow.Models.Activities;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class ExecuteSQLActivity : IActivity
	{
		public string Name { get; set; } = "executesqlact";
		public string WorkerID { get; set; } = "default";
		public string SQL { get; set; }
		public int ResultTable { get; set; } = 0;
		public Dictionary<string, string> ResultMap { get; set; }

		public ExecuteSQLActivity(string name, string workerId, string sql, int resultTable, Dictionary<string, string> resultMap)
		{
			Name = name;
			WorkerID = workerId;
			SQL = sql;
			ResultTable = resultTable;
			ResultMap = resultMap;
		}

		public IActivity Clone()
		{
			var resultMap = new Dictionary<string, string>();
			foreach (var key in ResultMap.Keys)
				resultMap.Add(key, ResultMap[key]);
			return new ExecuteSQLActivity(Name, WorkerID, SQL, ResultTable, resultMap);
		}
	}
}
