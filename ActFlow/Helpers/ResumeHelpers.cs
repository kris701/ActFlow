using ActFlow.Models.Workflows;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToolsSharp;

namespace ActFlow.Helpers
{
	internal static class ResumeHelpers
	{
		internal static async Task<List<WorkflowState>> GetResumableStates(string[] dirs, ILogger? logger)
		{
			var resumable = new List<WorkflowState>();
			foreach (var dir in dirs)
			{
				var stateFile = Path.Combine(dir, "state.json");
				if (!File.Exists(stateFile))
				{
					DirectoryHelper.DeleteDirectory(dir);
					continue;
				}

				try
				{
					var stateText = await File.ReadAllTextAsync(stateFile);
					var state = JsonSerializer.Deserialize<WorkflowState>(stateText, Constants.SerializerOpts);
					if (state == null)
					{
						DirectoryHelper.DeleteDirectory(dir);
						continue;
					}
					resumable.Add(state);
				}
				catch (Exception ex)
				{
					logger?.LogError($"Error, a old workflow state in the path '{dir}' has an invalid state file: {ex.Message}");
					DirectoryHelper.DeleteDirectory(dir);
				}
			}
			return resumable;
		}
	}
}
