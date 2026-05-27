using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class InsertWorkflowFromDatabaseActivity : IActivity
	{
		public string Name { get; set; } = "insertworkflow";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string TargetSTP { get; set; }
		[Required]
		public Dictionary<string, string> Arguments { get; set; }

		public bool HasInserted { get; set; } = false;
	}
}
