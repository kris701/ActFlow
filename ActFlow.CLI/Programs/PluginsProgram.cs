using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;

namespace ActFlow.CLI.Programs
{
	public static class PluginsProgram
	{
		public static async Task Run(PluginsOptions opts)
		{
			switch (opts.Action.ToLower())
			{
				case "add":
					if (Directory.Exists(Path.Combine(Constants._pluginPath, opts.PluginName + "." + opts.PluginVersion)))
						Console.WriteLine("Plugin already installed!");
					else
					{
						var loader = new NuGetLoader();
						await loader.LoadExtensions(opts.PluginName, opts.PluginVersion);
						Console.WriteLine($"Plugin '{opts.PluginName}' version '{opts.PluginVersion}' downloaded and ready for use!");
					}
					break;
				case "remove":
					var path = Path.Combine(Constants._pluginPath, opts.PluginName + "." + opts.PluginVersion);
					if (!Directory.Exists(path))
						Console.WriteLine("Plugin does not exist!");
					else
					{
						DirectoryHelpers.DeleteDirectory(path);
						Console.WriteLine("Plugin removed!");
					}
					break;
				case "list":
					var dirs = Directory.GetDirectories(Constants._pluginPath);
					foreach (var dir in dirs)
						Console.WriteLine("\tPackage: " + dir);
					if (dirs.Length == 0)
						Console.WriteLine("No packages installed!");
					break;
				default:
					Console.WriteLine("Unknown action!");
					break;
			}
		}
	}
}
