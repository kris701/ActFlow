using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;

namespace ActFlow.Helpers
{
	internal static class HumanInputHelpers
	{
		internal static void ApplyInput(Guid stateId, IContext input, List<WorkflowState> activeWorkflows)
		{
			var state = activeWorkflows.FirstOrDefault(x => x.ID == stateId);
			if (state != null)
			{
				var currentAct = state.Workflow.Activities[state.ActivityIndex];
				if (currentAct is not IHumanInput humanAct)
					throw new Exception($"Can only update the workflow on activities that implements '{nameof(IHumanInput)}'");
				if (state.Status != WorkflowStatuses.AwaitingHumanInput)
					throw new Exception("Current state is not awaiting any updates!");

				state.AppendToLog(WorkflowLogTypes.Warn, "Applying human input");
				humanAct.Apply(input);
			}
			else
				throw new Exception($"No workflow log item with the id '{stateId}' found!");
		}
	}
}
