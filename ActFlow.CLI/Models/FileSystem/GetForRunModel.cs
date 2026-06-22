using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActFlow.CLI.Models.FileSystem
{
	public class GetForRunModel
	{
		[Required]
		public Guid ID { get; set; }
	}
}
