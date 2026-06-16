namespace ActFlow.CLI.Models
{
	public class ConfigPluginsResult
	{
		public int ActFlowPluginCount { get; set; }
		public List<string> ActFlowPlugins { get; set; }

		public int PluginDependencyCount { get; set; }
		public List<string> PluginDependencies { get; set; }
	}
}
