using ActFlow.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow
{
	public static class Constants
	{
		public static JsonSerializerOptions SerializerOpts = new JsonSerializerOptions()
		{
			WriteIndented = true,
			TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo),
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			NumberHandling =
			JsonNumberHandling.AllowReadingFromString |
			JsonNumberHandling.WriteAsString
		};
	}
}
