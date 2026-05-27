using System.Reflection;
using System.Runtime.Loader;
using ToolsSharp;

namespace ActFlow.CLI.Helpers
{
	public static class PluginLoader
	{
		private static readonly List<string> _preferedNetVersion = new List<string>()
		{
			"net10.0",
			"net9.0",
			"net8.0",
			"netstandard2.1",
			"netstandard2.0",
			"native"
		};

		public static void LoadPlugins()
		{
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, StaticDirectories._pluginPath)))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, StaticDirectories._pluginPath));

			var plugins = Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, StaticDirectories._pluginPath));
			ConsoleHelpers.WriteLineColor($"\t{plugins.Length} plugins to load", ConsoleColor.DarkGray);
			var loaded = 0;
			foreach (var plugin in plugins)
			{
				try
				{
					var dlls = new List<string>();
					dlls.AddRange(GetRuntimes(plugin));
					dlls.AddRange(GetLib(plugin));
					foreach (var file in dlls)
					{
						var assemblyName = AssemblyName.GetAssemblyName(file);
						var assemblies = AppDomain.CurrentDomain.GetAssemblies();
						if (!assemblies.Any(x => x.GetName().Name == assemblyName.Name))
						{
							AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
							loaded++;
						}
					}
				}
				catch (Exception ex)
				{
					ConsoleHelpers.WriteLineColor($"\tError loading '{plugin}': {ex.Message}", ConsoleColor.Red);
				}
			}
			ConsoleHelpers.WriteLineColor($"\tLoaded {loaded} out of {plugins.Length}", ConsoleColor.DarkGray);
		}

		private static List<string> GetRuntimes(string plugin)
		{
			var targetLib = Path.Combine(plugin, "runtimes");
			if (!Directory.Exists(targetLib))
				return new List<string>();

			var runtimeDir = new DirectoryInfo(targetLib);
			var runtimeDirs = runtimeDir.GetDirectories();
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				var platformTarget = runtimeDirs.FirstOrDefault(x => x.FullName.EndsWith("win"));
				if (platformTarget == null)
					platformTarget = runtimeDirs.FirstOrDefault(x => x.FullName.EndsWith("win-x64"));
				if (platformTarget == null)
					return new List<string>();

				if (Directory.Exists(Path.Combine(platformTarget.FullName, "lib")))
					targetLib = Path.Combine(platformTarget.FullName, "lib");
				else
					targetLib = platformTarget.FullName;
			}
			else if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				var platformTarget = runtimeDirs.FirstOrDefault(x => x.FullName.EndsWith("unix"));
				if (platformTarget == null)
					return new List<string>();
				targetLib = Path.Combine(platformTarget.FullName, "lib");
			}

			var libDirs = Directory.GetDirectories(targetLib).ToList();
			libDirs.RemoveAll(x => !_preferedNetVersion.Any(y => x.EndsWith(y)));
			string? first = null;
			foreach (var version in _preferedNetVersion)
			{
				var target = libDirs.FirstOrDefault(x => x.EndsWith(version));
				if (target != null)
				{
					first = target;
					break;
				}
			}
			if (first == null)
				return new List<string>();

			if (first.EndsWith("native"))
			{
				MoveNativeDLLs(first);
				return new List<string>();
			}
			else
				return GetDlls(first);
		}

		private static List<string> GetLib(string plugin)
		{
			var targetLib = Path.Combine(plugin, "lib");
			if (!Directory.Exists(targetLib))
				return new List<string>();

			var libDirs = Directory.GetDirectories(targetLib).ToList();
			libDirs.RemoveAll(x => !_preferedNetVersion.Any(y => x.EndsWith(y)));
			string? first = null;
			foreach (var version in _preferedNetVersion)
			{
				var target = libDirs.FirstOrDefault(x => x.EndsWith(version));
				if (target != null)
				{
					first = target;
					break;
				}
			}
			if (first == null)
				return new List<string>();

			return GetDlls(first);
		}

		private static List<string> GetDlls(string folder)
		{
			var libs = new List<string>();

			var dir = new DirectoryInfo(folder);
			var files = dir.GetFiles();
			foreach (var file in files)
			{
				if (file.Extension.ToLower() != ".dll")
					continue;
				libs.Add(file.FullName);
			}

			return libs;
		}

		private static void MoveNativeDLLs(string folder)
		{
			var dir = new DirectoryInfo(folder);
			var files = dir.GetFiles();
			foreach (var file in files)
			{
				if (file.Extension.ToLower() != ".dll")
					continue;
				var path = Path.Combine(Environment.CurrentDirectory, file.Name);
				if (!File.Exists(path))
					File.Copy(file.FullName, path);
			}
		}
	}
}
