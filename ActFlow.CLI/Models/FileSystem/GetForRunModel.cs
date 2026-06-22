using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class GetForRunModel
	{
		[Required]
		public Guid ID { get; set; }
	}
}
