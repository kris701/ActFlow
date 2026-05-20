using ActFlow;
using ActFlow.CLI;
using ActFlow.Extensions;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using CommandLine;
using CommandLine.Text;
using NuGet;
using NuGet.Protocol.Core.Types;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

internal class Program
{
	private static JsonSerializerOptions _serializerOpts = new JsonSerializerOptions() { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo) };

	static async Task Main(string[] args)
	{
		var parser = new Parser(with => with.HelpWriter = null);
		var parserResult = parser.ParseArguments<Options>(args);
		parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
		await parserResult.WithParsedAsync(Run);
	}

	public static async Task Run(Options opts)
	{
		Console.WriteLine("Checking paths...");
		var inputFile = File.ReadAllText(opts.InputPath);
		var configFile = File.ReadAllText(opts.ConfigPath);
		var packageFile = File.ReadAllText(opts.PackagePath);

		Console.WriteLine("Ensuring packages are installed...");
		var targetPackages = JsonSerializer.Deserialize<List<string>>(packageFile);
		if (targetPackages == null)
			throw new Exception("Packages is malformed!");

		var loader = new NuGetLoader();
		await loader.LoadExtensions(targetPackages);
		var plugins = Directory.GetDirectories(".plugins");
		var found = 0;
		foreach(var plugin in plugins)
		{
			var target = targetPackages.FirstOrDefault(x => plugin.StartsWith(Path.Combine(".plugins", x)));
			if (target != null)
			{
				var targetPath = Path.Combine(plugin, "lib", "net10.0", target + ".dll");
				if (!File.Exists(targetPath))
					throw new Exception($"Could not find package '{target}'");
				Assembly.LoadFrom(targetPath);
				AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(targetPath));
				found++;
			}
		}
		if (found != targetPackages.Count)
			throw new Exception("Could not find all packages!");

		Console.WriteLine("Parsing Config...");
		var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, _serializerOpts);
		if (workers == null)
			throw new Exception("Config is malformed!");

		Console.WriteLine("Parsing input workflow...");
		var workflow = JsonSerializer.Deserialize<Workflow>(inputFile, _serializerOpts);
		if (workflow == null)
			throw new Exception("Input workflow is malformed!");

		Console.WriteLine("Initializing engine...");
		IActFlowEngine engine = new ActFlowEngine(workers);

		Console.WriteLine("Executing workflow...");
		var result = await engine.ExecuteAsync(workflow);

		Console.WriteLine("Workflow completed! Outputting result...");
		File.WriteAllText(opts.OutputPath, JsonSerializer.Serialize(result, _serializerOpts));
	}

	private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
	{
		var helpText = HelpText.AutoBuild(result, h =>
		{
			h.AddEnumValuesToHelpText = true;
			return h;
		}, e => e, verbsIndex: true);
		Console.WriteLine(helpText);
		HandleParseError(errs);
	}

	private static void HandleParseError(IEnumerable<Error> errs)
	{
		var sentenceBuilder = SentenceBuilder.Create();
		foreach (var error in errs)
			if (error is not HelpRequestedError)
				Console.WriteLine(sentenceBuilder.FormatError(error));
	}
}