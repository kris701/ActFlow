using ActFlow.Tools.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow.TestTools
{
	public static class SerializationHelpers
	{
		public static void SerializeTest<T>(T input)
		{
			// ARRANGE
			var options = new JsonSerializerOptions()
			{
				TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo)
			};
			var expectedText = JsonSerializer.Serialize(input, options);
			Assert.IsNotNull(expectedText);

			// ACT
			var actual = JsonSerializer.Deserialize<T>(expectedText, options);
			var actualText = JsonSerializer.Serialize(actual, options);

			// ASSERT
			Assert.StartsWith("{\"$type\":", expectedText);
			Assert.StartsWith("{\"$type\":", actualText);
			Assert.AreEqual(expectedText, actualText);
		}
	}
}
