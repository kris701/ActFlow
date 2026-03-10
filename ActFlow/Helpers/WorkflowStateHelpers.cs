using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Helpers
{
	public static class WorkflowStateHelpers
	{
		public static int FindNextActivityIndex(WorkflowState state, WorkerResult executionResult)
		{
			var nextActivityIndex = state.ActivityIndex + 1;
			if (executionResult.TargetActivity != "")
			{
				state.AppendToLog($"\tNext target activity is named {executionResult.TargetActivity}");
				nextActivityIndex = state.Workflow.Activities.FindIndex(x => x.Name == executionResult.TargetActivity);
				if (nextActivityIndex == -1)
					throw new Exception($"Could not find a activity named '{executionResult.TargetActivity}'");
				if (state.Workflow.Activities.Count(x => x.Name == executionResult.TargetActivity) > 1)
					state.AppendToLog($"Warning, multiple activities with the name '{executionResult.TargetActivity}' found! Using the first one...");
			}
			return nextActivityIndex;
		}

		public static void InsertResultIntoContextStore(WorkflowState state, string activityName, WorkerResult executionResult)
		{
			var values = executionResult.Context.GetContextValues();
			foreach (var valueKey in values.Keys)
				state.AddContext($"{activityName}.{valueKey}".ToLower(), values[valueKey]);
		}
	}
}
