using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class CreatePathModel
	{
		[Required]
		public string Path { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
