using System.Reflection;

namespace ActFlow.CLI.Helpers
{
	public static class PluginLoader
	{
		public static void LoadPlugins()
		{
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath)))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath));

			var plugins = Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, Constants._pluginPath));
			Console.WriteLine($"\t{plugins.Length} plugins to load");
			foreach (var plugin in plugins)
			{
				var lib = Path.Combine(plugin, "lib");
				if (!Directory.Exists(lib))
					continue;
				var libDirs = Directory.GetDirectories(lib);
				if (libDirs.Length == 0)
					continue;
				var first = libDirs.First();

				var dir = new DirectoryInfo(plugin);
				var files = dir.GetFiles();
				var target = files.FirstOrDefault(x => x.Name.EndsWith(".nuspec"));
				if (target == null)
					continue;
				var dllToLoad = Path.Combine(first, target.Name.Replace(".nuspec", "") + ".dll");

				var assemblyName = AssemblyName.GetAssemblyName(dllToLoad);
				if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == assemblyName.Name))
				{
					Assembly.LoadFrom(dllToLoad);
					AppDomain.CurrentDomain.Load(assemblyName);
				}
			}
		}
	}
}
