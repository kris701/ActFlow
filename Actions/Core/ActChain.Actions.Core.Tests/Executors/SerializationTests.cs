using ActChain.Actions.Core.Actions;
using ActChain.Actions.Core.Executors;
using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.Core.Tests.Executors
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ConditionalIfExecutor("a"),
				"TestFiles/Executors/ConditionalIfExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserExecutor("a", 1000),
				"TestFiles/Executors/ConditionalUserExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextExecutor("a"),
				"TestFiles/Executors/CreateContextExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsExecutor("a"),
				"TestFiles/Executors/InsertGlobalsExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new NoActionExecutor("a"),
				"TestFiles/Executors/NoActionExecutor_Serialization_Expected.json" };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActionExecutor input, string expectedFile)
		{
			// ARRANGE
			var options = new JsonSerializerOptions()
			{
				TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo)
			};

			// ACT
			var actualText = JsonSerializer.Serialize(input, options);

			// ASSERT
			var expectedFileText = File.ReadAllText(expectedFile);
			var expectedObject = JsonSerializer.Deserialize<IActionExecutor>(expectedFileText, options);
			var expectedText = JsonSerializer.Serialize(expectedObject, options);
			Assert.AreEqual(expectedText, actualText);
		}
	}
}
