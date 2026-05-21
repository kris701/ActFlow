using ActFlow.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow.CLI
{
	public class Constants
	{
		public static JsonSerializerOptions _serializerOpts = new JsonSerializerOptions() { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo) };
		public static string _pluginPath = ".plugins";
	}
}
