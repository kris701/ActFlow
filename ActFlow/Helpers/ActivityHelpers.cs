using ActFlow.Models.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ActFlow.Helpers
{
	public static class ActivityHelpers
	{
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);

		public static IActivity ApplyContexts(WorkflowState state, IActivity activity)
		{
			var text = JsonSerializer.Serialize(activity);
			var matches = _variableRegex.Matches(text);
			foreach (Match match in matches)
			{
				var key = match.Groups[1].Value.ToLower();
				if (state.ContextStore.Keys.Contains(key))
					text = text.Replace(match.Groups[0].Value, state.ContextStore[key]);
				else
					throw new Exception($"Variable '{key}' was not found in the current state!");
			}
			var deActivity = JsonSerializer.Deserialize<IActivity>(text);
			if (deActivity == null)
				throw new ArgumentNullException("Could not deserialize the activity!");
			return deActivity;
		}

		public static async Task<WorkerResult> ExecuteActivityAsync<T>(BaseWorker<T> worker, T act, WorkflowState state, CancellationToken token, string tmpDirectory) where T : IActivity
		{
			return await worker.Execute(act, state, token, tmpDirectory);
		}
	}
}
