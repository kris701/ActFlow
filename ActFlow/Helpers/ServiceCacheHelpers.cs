using ActFlow.Models;
using ActFlow.Models.Workers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.Helpers
{
	internal static class ServiceCacheHelpers
	{
		internal static Dictionary<ServiceKey, IWorker> GenerateServiceCache(List<IWorker> workers, string persistent)
		{
			var serviceCache = new Dictionary<ServiceKey, IWorker>();
			foreach (var worker in workers)
			{
				worker.PersistenDirectory = persistent;

				var tpy = worker.GetType();
				var baseType = tpy.BaseType;
				var typeArg = baseType?.GenericTypeArguments[0].Name;
				var key = new ServiceKey(worker.ID, typeArg);
				if (!serviceCache.ContainsKey(key))
					serviceCache.Add(key, worker);
			}
			return serviceCache;
		}
	}
}
