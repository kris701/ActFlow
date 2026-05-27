using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class InsertGlobalsActivity : IActivity
	{
		public string Name { get; set; } = "insertglobal";
		public string WorkerID { get; set; } = "default";
		[Required]
		public Dictionary<string, string> Arguments { get; set; }
	}
}
