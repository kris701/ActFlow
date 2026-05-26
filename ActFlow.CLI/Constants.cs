using ActFlow.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow.CLI
{
	public class Constants
	{
		public static JsonSerializerOptions _serializerOpts = new JsonSerializerOptions() { 
			WriteIndented = true, 
			TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo),
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			NumberHandling =
					JsonNumberHandling.AllowReadingFromString |
					JsonNumberHandling.WriteAsString
		};
		public static string _pluginPath = ".plugins";
	}
}
