using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class ExecuteSTPActivity : IActivity
	{
		public string Name { get; set; } = "fetchitemsfromdatabases";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string TargetSTP { get; set; }
		[Required]
		public Dictionary<string, string> Arguments { get; set; }
		public int ResultTable { get; set; } = 0;
		[Required]
		public Dictionary<string, string> ResultMap { get; set; }

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			var resultMap = new Dictionary<string, string>();
			foreach (var key in ResultMap.Keys)
				resultMap.Add(key, ResultMap[key]);
			return new ExecuteSTPActivity() {
				Name = Name,
				WorkerID = WorkerID,
				TargetSTP = TargetSTP,
				Arguments = arguments,
				ResultTable = ResultTable,
				ResultMap = resultMap
			};
		}
	}
}
