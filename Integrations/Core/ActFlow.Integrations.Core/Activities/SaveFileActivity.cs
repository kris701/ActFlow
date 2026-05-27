using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class SaveFileActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		[Required]
		public string Data { get; set; }
		[Required]
		public string Path { get; set; }
		[Required]
		public FileDirectories Directory { get; set; }
	}
}
