using ActFlow.Models.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.Helpers
{
	internal static class CancellationHelpers
	{
		internal static async Task RequestCancelIfExists(Guid id, List<WorkflowState> activeWorkflows)
		{
			var target = activeWorkflows.FirstOrDefault(x => x.ID == id);
			if (target != null)
			{
				target.AppendToLog(WorkflowLogTypes.Warn, "Cancellation requested");
				target.TokenSource.Cancel();
				while (activeWorkflows.Contains(target))
					await Task.Delay(500);
				target.Status = WorkflowStatuses.Canceled;
				await target.Update();
			}
		}
	}
}
