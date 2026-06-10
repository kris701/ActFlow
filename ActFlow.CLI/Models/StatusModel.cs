using ActFlow.Models.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class StatusModel
	{
		public int ActiveWorkflows { get; set; }
		public int ArchivedWorkflows { get; set; }

		public StatusModelRun? MostExpensiveRun { get; set; }
		public StatusModelRun? LeastExpensiveRun { get; set; }

		public TimeSpan TotalRuntime { get; set; }

		public DateTime? OldestRun { get; set; }
		public DateTime? LatestRun { get; set; }

		public Dictionary<WorkflowStatuses, int> ActiveStateMap { get; set; }
		public Dictionary<WorkflowStatuses, int> ArchivedStateMap { get; set; }
	}
}
