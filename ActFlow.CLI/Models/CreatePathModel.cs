using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class CreatePathModel
	{
		[Required]
		public string Path { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
