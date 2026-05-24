using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

using System.Text.Json;

namespace ActFlow.CLI.Programs
{
	public static class RunProgram
	{
		public static async Task Run(RunOptions opts)
		{
			Console.WriteLine("Checking paths...");
			var inputFile = File.ReadAllText(opts.InputPath);
			var configFile = File.ReadAllText(opts.ConfigPath);

			Console.WriteLine("Loading plugins...");
			PluginLoader.LoadPlugins();

			Console.WriteLine("Parsing Config...");
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants._serializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			Console.WriteLine("Parsing input workflow...");
			var workflow = JsonSerializer.Deserialize<Workflow>(inputFile, Constants._serializerOpts);
			if (workflow == null)
				throw new Exception("Input workflow is malformed!");

			Console.WriteLine("Initializing engine...");
			IActFlowEngine engine = new ActFlowEngine(workers)
			{
				ActivityLimiter = opts.Limiter,
				PersistentDirectory = opts.PersistentDirectory,
				TemporaryDirectory = opts.RunnerDirectory
			};

			Console.WriteLine("Executing workflow...");
			var result = await engine.ExecuteAsync(workflow);

			Console.WriteLine("Workflow completed! Outputting result...");
			File.WriteAllText(opts.OutputPath, JsonSerializer.Serialize(result, Constants._serializerOpts));
		}
	}
}
