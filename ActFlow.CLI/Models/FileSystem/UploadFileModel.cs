using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Models.FileSystem
{
	public class UploadFileModel
	{
		[Required]
		public string Path { get; set; }
		[Required]
		public IFormFile File { get; set; }
	}
}
