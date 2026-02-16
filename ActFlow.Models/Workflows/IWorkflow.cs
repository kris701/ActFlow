using ActChain.Models.Activities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActChain.Models.Workflows
{
	public interface IWorkflow
	{
		public string Name { get; set; }
		public Dictionary<string, string> Globals { get; set; }
		public List<IActivity> Stages { get; set; }
	}
}
