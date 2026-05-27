using ActFlow.Archiver;
using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;
using ToolsSharp;

namespace ActFlow.CLI.Programs
{
	public static class ArchiverProgram
	{
		public static async Task Run(ArchiverOptions opts)
		{
			ConsoleHelpers.WriteLineColor("Initializing archive...", ConsoleColor.Blue);
			var archiver = new WorkflowArchive() { 
				CompletedDirectory = opts.CompletedDirectory
			};
			await archiver.Initialize();

			switch (opts.Action.ToLower())
			{
				case "get":
					ConsoleHelpers.WriteLineColor("Loading plugins...", ConsoleColor.Blue);
					PluginLoader.LoadPlugins();

					var getItem = archiver.GetCompletedWorkflow(opts.WorkflowRunID);
					ConsoleHelpers.WriteLineColor($"\tID: {getItem.State.ID}", ConsoleColor.DarkGray);
					ConsoleHelpers.WriteLineColor($"\tName: {getItem.State.Workflow.Name}", ConsoleColor.DarkGray);
					ConsoleHelpers.WriteLineColor($"\tStatus: {Enum.GetName(getItem.State.Status)}", ConsoleColor.DarkGray);
					if (getItem.Files.Count > 0)
					{
						ConsoleHelpers.WriteLineColor($"\tTemporary files", ConsoleColor.DarkGray);
						foreach (var file in getItem.Files)
							ConsoleHelpers.WriteLineColor($"\t\t{file}", ConsoleColor.DarkGray);
					}
					break;
				case "remove":
					archiver.RemoveCompletedWorkflow(opts.WorkflowRunID);
					ConsoleHelpers.WriteLineColor("Archived workflow run deleted!", ConsoleColor.Green);
					break;
				case "list":
					var items = archiver.GetAllCompletedWorkflows();
					if (items.Count == 0)
					{
						ConsoleHelpers.WriteLineColor("There are no completed workflows...", ConsoleColor.DarkYellow);
					}
					else
					{
						ConsoleHelpers.WriteLineColor("Completed workflows:", ConsoleColor.Blue);
						if (items.Count > 1000)
						{
							ConsoleHelpers.WriteLineColor("There are more than 1000 completed workflows! Limiting display to first 1000...", ConsoleColor.DarkYellow);
							items = items.Take(1000).ToList();
						}
						foreach (var item in items)
							ConsoleHelpers.WriteLineColor($"\t[{item.ID}] ({Enum.GetName(item.Status)}) {item.Name}, started {item.StartedAt}, ended {item.EndedAt}", ConsoleColor.DarkGray);
					}
					break;
				default:
					ConsoleHelpers.WriteLineColor("Unknown action!", ConsoleColor.Red);
					break;
			}
		}
	}
}
