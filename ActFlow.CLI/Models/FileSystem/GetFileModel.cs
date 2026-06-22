using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class GetFileModel
	{
		[Required]
		public string Path { get; set; }
	}
}
