using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class DeleteDirectoryModel
	{
		[Required]
		public string Path { get; set; }
	}
}
