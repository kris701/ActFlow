using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Reflection;
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
			var plugins = Directory.GetDirectories(Constants._pluginPath);
			Console.WriteLine($"\t{plugins.Length} plugins to load");
			foreach (var plugin in plugins)
			{
				var lib = Path.Combine(plugin, "lib");
				if (!Directory.Exists(lib))
					continue;
				var libDirs = Directory.GetDirectories(lib);
				if (libDirs.Length == 0)
					continue;
				var first = libDirs.First();

				var files = Directory.GetFiles(plugin);
				var target = files.FirstOrDefault(x => x.EndsWith(".nuspec"));
				if (target == null)
					continue;
				target = target.Substring(target.LastIndexOf('\\') + 1);
				var actualName = target.Substring(0, target.LastIndexOf(".nuspec"));
				var dllToLoad = Path.Combine(first, actualName + ".dll");

				var assemblyName = AssemblyName.GetAssemblyName(dllToLoad);
				if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == assemblyName.Name))
					throw new Exception($"Assembly with name '{assemblyName.Name}' already loaded!");
				Assembly.LoadFrom(dllToLoad);
				AppDomain.CurrentDomain.Load(assemblyName);
			}

			Console.WriteLine("Parsing Config...");
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants._serializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			Console.WriteLine("Parsing input workflow...");
			var workflow = JsonSerializer.Deserialize<Workflow>(inputFile, Constants._serializerOpts);
			if (workflow == null)
				throw new Exception("Input workflow is malformed!");

			Console.WriteLine("Initializing engine...");
			IActFlowEngine engine = new ActFlowEngine(workers);

			Console.WriteLine("Executing workflow...");
			var result = await engine.ExecuteAsync(workflow);

			Console.WriteLine("Workflow completed! Outputting result...");
			File.WriteAllText(opts.OutputPath, JsonSerializer.Serialize(result, Constants._serializerOpts));
		}
	}
}
