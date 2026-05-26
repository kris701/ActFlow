using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class ListFilesActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		[Required]
		public string Path { get; set; }
		[Required]
		public FileDirectories Directory { get; set; }

		public IActivity Clone() => new ListFilesActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			Path = Path,
			Directory = Directory
		};
	}
}
