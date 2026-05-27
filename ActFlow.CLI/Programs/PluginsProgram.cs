using ActFlow.CLI.Models;
using ToolsSharp;

namespace ActFlow.CLI.Programs
{
	public static class PluginsProgram
	{
		public static async Task Run(PluginsOptions opts)
		{
			switch (opts.Action.ToLower())
			{
				case "add":
					if (Directory.Exists(Path.Combine(StaticDirectories._pluginPath, opts.PluginName + "." + opts.PluginVersion)))
						ConsoleHelpers.WriteLineColor("Plugin already installed!", ConsoleColor.DarkYellow);
					else
					{
						var loader = new NuGetLoader();
						await loader.LoadExtensions(opts.PluginName, opts.PluginVersion);
						ConsoleHelpers.WriteLineColor($"Plugin '{opts.PluginName}' version '{opts.PluginVersion}' downloaded and ready for use!", ConsoleColor.Green);
					}
					break;
				case "remove":
					var path = Path.Combine(StaticDirectories._pluginPath, opts.PluginName + "." + opts.PluginVersion);
					if (!Directory.Exists(path))
						ConsoleHelpers.WriteLineColor("Plugin does not exist!", ConsoleColor.Red);
					else
					{
						DirectoryHelper.DeleteDirectory(path);
						ConsoleHelpers.WriteLineColor("Plugin removed!", ConsoleColor.Green);
					}
					break;
				case "list":
					var dirs = Directory.GetDirectories(StaticDirectories._pluginPath);
					foreach (var dir in dirs)
						ConsoleHelpers.WriteLineColor("\tPackage: " + dir, ConsoleColor.DarkGray);
					if (dirs.Length == 0)
						ConsoleHelpers.WriteLineColor("No packages installed!", ConsoleColor.DarkYellow);
					break;
				default:
					ConsoleHelpers.WriteLineColor("Unknown action!", ConsoleColor.Red);
					break;
			}
		}
	}
}
