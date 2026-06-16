using ActFlow.CLI.Models;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using Microsoft.AspNetCore.Mvc;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Controller endpoints to get current configuration of the ActFlow engine
	/// </summary>
	[ApiController]
	[Route("api/config")]
	public class ConfigController : ControllerBase
	{
		private readonly IActFlowEngine _engine;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		public ConfigController(IActFlowEngine engine)
		{
			_engine = engine;
		}

		/// <summary>
		/// Get a list of plugins that are installed
		/// </summary>
		/// <returns></returns>
		[HttpGet("plugins")]
		public async Task<ActionResult<ConfigPluginsResult>> Get_Plugins()
		{
			var plugins = Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, StaticDirectories._pluginPath));
			var pluginNames = new List<string>();
			foreach (var plugin in plugins)
			{
				var dirInfo = new DirectoryInfo(plugin);
				pluginNames.Add(dirInfo.Name);
			}

			var actFlowPlugins = pluginNames.Where(x => x.StartsWith("ActFlow.Integrations.")).ToList();
			var dependencyPlugins = pluginNames.Where(x => !actFlowPlugins.Contains(x)).ToList();

			var result = new ConfigPluginsResult()
			{
				ActFlowPluginCount = actFlowPlugins.Count,
				ActFlowPlugins = actFlowPlugins,
				PluginDependencyCount = dependencyPlugins.Count,
				PluginDependencies = dependencyPlugins,
			};

			return Ok(result);
		}

		/// <summary>
		/// Get all contexts that can be used
		/// </summary>
		/// <returns></returns>
		[HttpGet("contexts")]
		public async Task<ActionResult<List<IContext>>> Get_Contexts()
		{
			var result = new List<IContext>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var contextTypes = assemblies.SelectMany(s => s.GetTypes())
					.Where(p => typeof(IContext).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

			foreach (var type in contextTypes)
				result.Add((IContext)Activator.CreateInstance(type));

			return Ok(result);
		}

		/// <summary>
		/// Get all activities that can be used
		/// </summary>
		/// <returns></returns>
		[HttpGet("activities")]
		public async Task<ActionResult<List<IActivity>>> Get_Activities()
		{
			var result = new List<IActivity>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var contextTypes = assemblies.SelectMany(s => s.GetTypes())
					.Where(p => typeof(IActivity).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

			foreach (var type in contextTypes)
			{
				var item = Activator.CreateInstance(type);
				if (item is IActivity act)
				{
					foreach(var worker in _engine.Workers)
					{
						var wType = worker.GetType();
						if (wType.BaseType != null && wType.BaseType.GenericTypeArguments.ToList().Contains(act.GetType()))
						{
							var clone = Activator.CreateInstance(type);
							if (clone is IActivity cAct)
							{
								cAct.WorkerID = worker.ID;
								result.Add(cAct);
							}
						}
					}
				}
			}

			return Ok(result);
		}

		/// <summary>
		/// Get all workers currently configured
		/// </summary>
		/// <returns>A list of worker type and IDs</returns>
		[HttpGet("workers")]
		public async Task<ActionResult<List<ConfigWorkersResult>>> Get_Workers()
		{
			var result = new List<ConfigWorkersResult>();

			foreach (var worker in _engine.Workers)
				result.Add(new ConfigWorkersResult(worker.GetType().Name, worker.ID));

			return Ok(result);
		}
	}
}
