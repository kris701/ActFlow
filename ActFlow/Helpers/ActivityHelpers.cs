using ActFlow.Models.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.RegularExpressions;

namespace ActFlow.Helpers
{
	public static class ActivityHelpers
	{
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);
		public static IActivity ApplyContexts(WorkflowState state, IActivity activity)
		{
			var props = activity.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (prop.PropertyType == typeof(string))
				{
					var value = prop.GetValue(activity);
					if (value is string str)
					{
						var newStr = str;
						var matches = _variableRegex.Matches(str);
						foreach (Match match in matches)
						{
							var key = match.Groups[1].Value.ToLower();
							if (state.ContextStore.Keys.Contains(key))
								newStr = newStr.Replace(match.Groups[0].Value, state.ContextStore[key]);
							else
								throw new Exception($"Variable '{key}' was not found in the current state!");
						}

						if (newStr != str)
							prop.SetValue(activity, newStr);
					}
				}
			}

			return activity;
		}

		public static async Task<WorkerResult> ExecuteActivityAsync<T>(BaseWorker<T> worker, T act, WorkflowState state, CancellationToken token, string tmpDirectory) where T : IActivity
		{
			return await worker.Execute(act, state, token, tmpDirectory);
		}
	}
}
