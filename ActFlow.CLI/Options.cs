using CommandLine;

namespace ActFlow.CLI
{
	public class Options
	{
		[Option('i', "input", Required = true, HelpText = "Target input file to run", Default = "")]
		public string InputPath { get; set; } = "";
		[Option('c', "config", Required = true, HelpText = "Target config file to use. This should be a JSON file, being a list of IWorker configs", Default = "")]
		public string ConfigPath { get; set; } = "";
		[Option('o', "output", Required = false, HelpText = "Target file to output to", Default = "out.json")]
		public string OutputPath { get; set; } = "out.json";
	}
}
