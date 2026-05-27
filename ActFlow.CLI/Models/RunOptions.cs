using CommandLine;

namespace ActFlow.CLI.Models
{
	[Verb("run", HelpText = "Run a workflow file.")]
	public class RunOptions
	{
		[Value(0, HelpText = "Target input file to run", MetaName = "Input", Required = true)]
		public string InputPath { get; set; } = "";

		[Option('c', "config", Required = true, HelpText = "Target config file to use. This should be a JSON file, being a list of IWorker configs", Default = "")]
		public string ConfigPath { get; set; } = "";
		[Option('o', "output", Required = false, HelpText = "Target file to output to", Default = "out.json")]
		public string OutputPath { get; set; } = "out.json";

		[Option("limiter", Required = false, HelpText = "Limit how many activities are allowed to run for a single workflow", Default = 100)]
		public int Limiter { get; set; } = 100;
		[Option("persistent", Required = false, HelpText = "Directory to keep persistent data in", Default = ".persistent")]
		public string PersistentDirectory { get; set; } = ".persistent";
		[Option("runner", Required = false, HelpText = "Directory to keep active workflow runs in", Default = ".runners")]
		public string RunnerDirectory { get; set; } = ".runners";
		[Option("completed", Required = false, HelpText = "Directory to store completed workflows", Default = ".completed")]
		public string CompletedDirectory { get; set; } = ".completed";
	}
}
