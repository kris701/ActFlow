using CommandLine;

namespace ActFlow.CLI.Models
{
	[Verb("archive", HelpText = "Interact with older workflow runs with the Archiver.")]
	public class ArchiverOptions
	{
		[Value(0, HelpText = "What action type to use on the archive. Options are 'get', 'remove' and 'list'", MetaName = "Action", Required = true)]
		public string Action { get; set; } = "";

		[Value(1, HelpText = "Workflow run id", MetaName = "Workflow run id")]
		public Guid WorkflowRunID { get; set; } = Guid.Empty;

		[Option("completed", Required = false, HelpText = "Directory to store completed workflows", Default = ".completed")]
		public string CompletedDirectory { get; set; } = ".completed";
	}
}
