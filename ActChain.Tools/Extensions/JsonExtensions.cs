using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Tools.Extensions
{
	public static class JsonExtensions
	{
		public static void AddNativePolymorphicTypInfo(JsonTypeInfo jsonTypeInfo)
		{
			if (jsonTypeInfo.Type == typeof(IAIAction))
				UpdateTypeInfo<IAIAction>(jsonTypeInfo);
			if (jsonTypeInfo.Type == typeof(IActionContext))
				UpdateTypeInfo<IActionContext>(jsonTypeInfo);
			if (jsonTypeInfo.Type == typeof(IActionExecutor))
				UpdateTypeInfo<IActionExecutor>(jsonTypeInfo);
		}

		private static void UpdateTypeInfo<T>(JsonTypeInfo jsonTypeInfo)
		{
			var actionTypes = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(s => s.GetTypes())
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
	}
}
