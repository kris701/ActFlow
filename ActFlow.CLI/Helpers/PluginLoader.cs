using System.Reflection;

namespace ActFlow.CLI.Helpers
{
	public static class PluginLoader
	{
		private static List<string> _preferedNetVersion = new List<string>()
		{
			"net10.0",
			"net9.0",
			"net8.0"
		};

		public static void LoadPlugins()
		{
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath)))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath));

			var plugins = Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath));
			Console.WriteLine($"\t{plugins.Length} plugins to load");
			foreach (var plugin in plugins)
			{
				try
				{
					var targetLib = "";
					var runtimes = Path.Combine(plugin, "runtimes");
					if (!Directory.Exists(runtimes))
					{
						var lib = Path.Combine(plugin, "lib");
						if (!Directory.Exists(lib))
							continue;
						targetLib = lib;
					}
					else
					{
						var runtimeDir = new DirectoryInfo(runtimes);
						var runtimeDirs = runtimeDir.GetDirectories();
						if (Environment.OSVersion.Platform == PlatformID.Win32NT)
						{
							var platformTarget = runtimeDirs.FirstOrDefault(x => x.FullName.EndsWith("win"));
							if (platformTarget == null)
								continue;
							targetLib = Path.Combine(platformTarget.FullName, "lib");
						}
						else if (Environment.OSVersion.Platform == PlatformID.Unix)
						{
							var platformTarget = runtimeDirs.FirstOrDefault(x => x.FullName.EndsWith("unix"));
							if (platformTarget == null)
								continue;
							targetLib = Path.Combine(platformTarget.FullName, "lib");
						}
					}

					var libDirs = Directory.GetDirectories(targetLib).ToList();
					libDirs.RemoveAll(x => !_preferedNetVersion.Any(y => x.EndsWith(y)));
					libDirs = libDirs.OrderByDescending(x => _preferedNetVersion.FindIndex(0, libDirs.Count, y => x.EndsWith(y))).ToList();
					if (libDirs.Count == 0)
						continue;
					var first = libDirs.First();

					var dir = new DirectoryInfo(plugin);
					var files = dir.GetFiles();
					var target = files.FirstOrDefault(x => x.Name.EndsWith(".nuspec"));
					if (target == null)
						continue;
					var dllToLoad = Path.Combine(first, target.Name.Replace(".nuspec", "") + ".dll");

					var assemblyName = AssemblyName.GetAssemblyName(dllToLoad);
					var assemblies = AppDomain.CurrentDomain.GetAssemblies();
					if (!assemblies.Any(x => x.GetName().Name == assemblyName.Name))
					{
						Assembly.LoadFrom(dllToLoad);
						AppDomain.CurrentDomain.Load(assemblyName);
					}
					else
					{

					}
				}
				catch(Exception ex)
				{
					Console.WriteLine($"\tError loading '{plugin}': {ex.Message}");
				}
			}
		}
	}
}
