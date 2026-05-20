using ActFlow;
using ActFlow.CLI;
using ActFlow.Extensions;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using CommandLine;
using CommandLine.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

internal class Program
{
	private static JsonSerializerOptions _serializerOpts = new JsonSerializerOptions() { TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo) };

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