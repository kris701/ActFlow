using ActFlow.CLI.Models;
using ActFlow.CLI.Programs;
using CommandLine;

internal class Program
{
	static async Task Main(string[] args)
	{
		var parser = new Parser(config => config.HelpWriter = Console.Out);
		var parserResult = parser.ParseArguments<RunOptions, PluginsOptions>(args);
		await parserResult.WithParsedAsync<PluginsOptions>(PluginsProgram.Run);
		await parserResult.WithParsedAsync<RunOptions>(RunProgram.Run);
	}
}