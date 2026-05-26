using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class ExecuteSQLActivity : IActivity
	{
		public string Name { get; set; } = "executesqlact";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string SQL { get; set; }
		public int ResultTable { get; set; } = 0;
		[Required]
		public Dictionary<string, string> ResultMap { get; set; }

		public IActivity Clone()
		{
			var resultMap = new Dictionary<string, string>();
			foreach (var key in ResultMap.Keys)
				resultMap.Add(key, ResultMap[key]);
			return new ExecuteSQLActivity() {
				Name = Name,
				WorkerID = WorkerID,
				SQL = SQL,
				ResultTable = ResultTable,
				ResultMap = resultMap
			};
		}
	}
}
