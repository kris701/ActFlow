using ActFlow.CLI.Models;
using ActFlow.CLI.Programs;
using CommandLine;
using System.Text.Json.Serialization;

namespace ActFlow.CLI
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			if (!Constants.SerializerOpts.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)) &&
				!Constants.SerializerOpts.Converters.IsReadOnly)
				Constants.SerializerOpts.Converters.Add(new JsonStringEnumConverter());

			var parser = new Parser(config => config.HelpWriter = Console.Out);
			var parserResult = parser.ParseArguments<RunOptions, PluginsOptions, ServeOptions, ArchiverOptions>(args);
			await parserResult.WithParsedAsync<PluginsOptions>(PluginsProgram.Run);
			await parserResult.WithParsedAsync<RunOptions>(RunProgram.Run);
			await parserResult.WithParsedAsync<ServeOptions>(ServeProgram.Run);
			await parserResult.WithParsedAsync<ArchiverOptions>(ArchiverProgram.Run);
		}
	}
}