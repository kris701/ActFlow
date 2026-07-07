using ActFlow.Models;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.RegularExpressions;

namespace ActFlow.Helpers
{
	internal static class ActivityHelpers
	{
		private static readonly Regex _variableRegex = new Regex("\\${{(.*?)}}", RegexOptions.Compiled);
		internal static IActivity ApplyContexts(WorkflowState state, IActivity activity)
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

		internal static async Task ExecuteActivityAsync(WorkflowState state, string tmpDirectory, Dictionary<ServiceKey, IWorker> serviceCache)
		{
			var activity = state.Workflow.Activities[state.ActivityIndex];
			state.AppendToLog($"Initializing activity {activity.Name}");

			activity = ApplyContexts(state, activity);

			state.AppendToLog($"Executing activity");
			await state.Update();
			var executionResult = await ExecuteActivityAsync(activity, state, state.TokenSource.Token, tmpDirectory, serviceCache);
			if (state.Status != WorkflowStatuses.Canceled)
				state.Status = WorkflowStatuses.Running;
			if (state.TokenSource.IsCancellationRequested)
				return;
			if (executionResult.Context is not EmptyContext)
			{
				state.AppendToLog($"Resulting activity context is a {executionResult.Context.GetType()}");
				var contextString = executionResult.Context.ToString();
				if (contextString != null)
					contextString = contextString.Length > 100 ? contextString.Substring(0, 100) : contextString;
				state.AppendToLog($"Resulting activity context content is {contextString}");
			}

			WorkflowStateHelpers.InsertResultIntoContextStore(state, activity.Name, executionResult);

			state.ActivityIndex = WorkflowStateHelpers.FindNextActivityIndex(state, executionResult);
		}

		private static async Task<WorkerResult> ExecuteActivityAsync(IActivity act, WorkflowState state, CancellationToken token, string tmpDirectory, Dictionary<ServiceKey, IWorker> serviceCache)
		{
			var targetKey = new ServiceKey(act.WorkerID, act.GetType().Name);
			if (serviceCache.ContainsKey(targetKey))
			{
				var executor = serviceCache[targetKey];
				return await ExecuteActivityAsync((dynamic)executor, (dynamic)act, state, token, tmpDirectory);
			}
			throw new Exception($"Unknown target activity executor '{targetKey}'! This probably means that the backend have not been set up to accept this type of action!");
		}

		private static async Task<WorkerResult> ExecuteActivityAsync<T>(BaseWorker<T> worker, T act, WorkflowState state, CancellationToken token, string tmpDirectory) where T : IActivity
		{
			return await worker.Execute(act, state, token, tmpDirectory);
		}
	}
}
