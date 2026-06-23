using ActFlow.Models.Workflows;
using System.Text.Json;
using ToolsSharp;

namespace ActFlow.Helpers
{
	internal static class CompletionHelpers
	{
		internal static async Task MoveCompletedWorkflow(WorkflowState state, string? completedDirectory, string runnerDirectory)
		{
			if (completedDirectory != null)
			{
				state.AppendToLog($"Moving workflow state to completed folder...");
				await state.Update();
				var path = Path.Combine(completedDirectory, state.ID.ToString());
				if (Directory.Exists(path))
					DirectoryHelper.DeleteDirectory(path);
				Directory.CreateDirectory(path);

				var workflowFile = Path.Combine(path, "state.json");
				await File.WriteAllTextAsync(workflowFile, JsonSerializer.Serialize(state, Constants.SerializerOpts));

				var orgTmpDirectory = Path.Combine(runnerDirectory, state.ID.ToString(), "tmp");
				var newTmpDirectory = Path.Combine(path, "tmp");
				if (!Directory.Exists(newTmpDirectory))
					Directory.CreateDirectory(newTmpDirectory);
				if (Directory.Exists(newTmpDirectory))
					Directory.CreateDirectory(newTmpDirectory);
				if (Directory.Exists(orgTmpDirectory))
					DirectoryHelper.CopyFilesRecursively(orgTmpDirectory, newTmpDirectory);
			}

			var tmpDirectory = Path.Combine(runnerDirectory, state.ID.ToString());
			if (Directory.Exists(tmpDirectory))
				DirectoryHelper.DeleteDirectory(tmpDirectory);
		}
	}
}
