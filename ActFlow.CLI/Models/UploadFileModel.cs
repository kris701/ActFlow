using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class UploadFileModel
	{
		[Required]
		public string Path { get; set; }
		[Required]
		public IFormFile File { get; set; }
	}
}
