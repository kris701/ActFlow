using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class ExecuteSQLToFileActivity : IActivity
	{
		public string Name { get; set; } = "executesqltofile";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string SQL { get; set; }
		public int ResultTable { get; set; } = 0;
		[Required]
		public string Path { get; set; }
		[Required]
		public FileDirectories Directory { get; set; }
	}
}
