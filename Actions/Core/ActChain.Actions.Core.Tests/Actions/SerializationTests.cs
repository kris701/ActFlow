using ActChain.Actions.Core.Actions;
using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using ActChain.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.Core.Tests.Actions
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new NoAction("abc", "a"),
				"TestFiles/NoAction_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalIfAction("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/ConditionalIfAction_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserAction("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/ConditionalUserAction_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextAction("abc", "a", new EmptyContext()),
				"TestFiles/CreateContextAction_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsAction("abc", "a", new Dictionary<string, string>() { { "a", "asdasdad" } }),
				"TestFiles/InsertGlobalsAction_Serialization_Expected.json" };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IAIAction input, string expectedFile)
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
			var expectedObject = JsonSerializer.Deserialize<IAIAction>(expectedFileText, options);
			var expectedText = JsonSerializer.Serialize(expectedObject, options);
			Assert.AreEqual(expectedText, actualText);
		}
	}
}
