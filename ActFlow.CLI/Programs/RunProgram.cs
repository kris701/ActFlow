using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;
using ToolsSharp;

namespace ActFlow.CLI.Programs
{
	public static class RunProgram
	{
		public static async Task Run(RunOptions opts)
		{
			ConsoleHelpers.WriteLineColor("Checking paths...", ConsoleColor.Blue);
			var inputFile = File.ReadAllText(opts.InputPath);
			var configFile = File.ReadAllText(opts.ConfigPath);

			ConsoleHelpers.WriteLineColor("Loading plugins...", ConsoleColor.Blue);
			PluginLoader.LoadPlugins();

			ConsoleHelpers.WriteLineColor("Parsing Config...", ConsoleColor.Blue);
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants.SerializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			ConsoleHelpers.WriteLineColor("Parsing input workflow...", ConsoleColor.Blue);
			var workflow = JsonSerializer.Deserialize<Workflow>(inputFile, Constants.SerializerOpts);
			if (workflow == null)
				throw new Exception("Input workflow is malformed!");

			ConsoleHelpers.WriteLineColor("Initializing engine...", ConsoleColor.Blue);
			IActFlowEngine engine = new ActFlowEngine(workers)
			{
				ActivityLimiter = opts.Limiter,
				PersistentDirectory = opts.PersistentDirectory,
				RunnerDirectory = opts.RunnerDirectory,
				CompletedDirectory = opts.CompletedDirectory,
			};
			await engine.Initialize();

			ConsoleHelpers.WriteLineColor("Executing workflow...", ConsoleColor.Blue);
			var result = await engine.ExecuteAsync(workflow);

			if (result.Status != WorkflowStatuses.Succeeded)
				ConsoleHelpers.WriteLineColor($"\tWorkflow did not succeed with the status '{Enum.GetName(result.Status)}'", ConsoleColor.Red);
			else
				ConsoleHelpers.WriteLineColor("\tWorkflow succeeded!", ConsoleColor.Green);

			ConsoleHelpers.WriteLineColor("Workflow completed! Outputting result...", ConsoleColor.Blue);
			File.WriteAllText(opts.OutputPath, JsonSerializer.Serialize(result, Constants.SerializerOpts));
		}
	}
}
