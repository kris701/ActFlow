using CommandLine;

namespace ActFlow.CLI.Models
{
	[Verb("plugins", HelpText = "Manage the plugins the engine can use.")]
	public class PluginsOptions
	{
		[Value(0, HelpText = "What action type to use on plugins. Options are 'add', 'remove' and 'list'", MetaName = "Action", Required = true)]
		public string Action { get; set; } = "";

		[Value(1, HelpText = "Plugin Name", MetaName = "Plugin")]
		public string PluginName { get; set; } = "";
		[Value(2, HelpText = "Plugin Version", MetaName = "Version")]
		public string PluginVersion { get; set; } = "";
	}
}
