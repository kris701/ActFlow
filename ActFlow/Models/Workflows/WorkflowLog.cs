using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.Models.Workflows
{
	public class WorkflowLog
	{
		public WorkflowLogTypes Type { get; set; }
		public string Text { get; set; }

		public WorkflowLog(WorkflowLogTypes type, string text)
		{
			Type = type;
			Text = text;
		}

		public WorkflowLog(string text)
		{
			Type = WorkflowLogTypes.Info;
			Text = text;
		}
	}
}
