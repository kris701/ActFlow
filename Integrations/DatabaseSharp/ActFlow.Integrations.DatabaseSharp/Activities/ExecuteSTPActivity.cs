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
	}
}
