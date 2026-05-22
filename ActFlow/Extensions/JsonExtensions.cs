using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ToolsSharp;

namespace ActFlow.Extensions
{
	public static class JsonExtensions
	{
		public static void AddNativePolymorphicTypInfo(JsonTypeInfo jsonTypeInfo)
		{
			if (jsonTypeInfo.Type == typeof(IActivity))
				UpdateTypeInfo<IActivity>(jsonTypeInfo);
			if (jsonTypeInfo.Type == typeof(IContext))
				UpdateTypeInfo<IContext>(jsonTypeInfo);
			if (jsonTypeInfo.Type == typeof(IWorker))
				UpdateTypeInfo<IWorker>(jsonTypeInfo);
		}

		private static void UpdateTypeInfo<T>(JsonTypeInfo jsonTypeInfo)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			assemblies.RemoveAll(x => !x.FullName.StartsWith("ActFlow"));
			var actionTypes = assemblies.SelectMany(s => s.GetTypes())
					.Where(p => typeof(T).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

			jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
			{
				TypeDiscriminatorPropertyName = "$type",
				IgnoreUnrecognizedTypeDiscriminators = true,
				UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
				DerivedTypes =
					{
					}
			};
			foreach (var item in actionTypes.Select(x => new JsonDerivedType(x, x.Name)))
				jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(item);
		}

		public static HashSet<string> GetAllPolymorphicTypes()
		{
			var types = new HashSet<string>();
			types.AddRange(GetTypeInfo<IActivity>());
			types.AddRange(GetTypeInfo<IContext>());
			types.AddRange(GetTypeInfo<IWorker>());
			return types;
		}

		private static List<string> GetTypeInfo<T>()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var actionTypes = assemblies.SelectMany(s => s.GetTypes())
					.Where(p => typeof(T).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

			return actionTypes.Select(x => x.Name).ToList();
		}
	}
}
