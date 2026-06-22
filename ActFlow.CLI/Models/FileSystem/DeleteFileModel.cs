using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class DeleteFileModel
	{
		[Required]
		public string Path { get; set; }
	}
}
