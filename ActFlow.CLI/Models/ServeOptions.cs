using CommandLine;

namespace ActFlow.CLI.Models
{
	[Verb("serve", HelpText = "Start a HTTP server that accepts workflow calls.")]
	public class ServeOptions
	{
		[Option('c', "config", Required = true, HelpText = "Target config file to use. This should be a JSON file, being a list of IWorker configs", Default = "")]
		public string ConfigPath { get; set; } = "";

		[Option('p', "port", Required = false, HelpText = "Port to listen on", Default = 5523)]
		public int Port { get; set; } = 5523;

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
