using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class GetFileForRunModel
	{
		[Required]
		public Guid ID { get; set; }
		[Required]
		public string Path { get; set; }
	}
}
