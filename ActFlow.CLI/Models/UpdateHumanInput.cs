using ActFlow.Models.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class UpdateHumanInput
	{
		public Guid ID { get; set; }
		public IContext Input { get; set; }
	}
}
