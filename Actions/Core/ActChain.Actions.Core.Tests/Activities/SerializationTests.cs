using ActChain.Actions.Core.Activities;
using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using ActChain.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.Core.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new NoAction("abc", "a"),
				"TestFiles/Activities/NoAction_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalIfAction("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/Activities/ConditionalIfAction_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserAction("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/Activities/ConditionalUserAction_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextAction("abc", "a", new EmptyContext()),
				"TestFiles/Activities/CreateContextAction_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsAction("abc", "a", new Dictionary<string, string>() { { "a", "asdasdad" } }),
				"TestFiles/Activities/InsertGlobalsAction_Serialization_Expected.json" };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input, string expectedFile)
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
			var expectedObject = JsonSerializer.Deserialize<IActivity>(expectedFileText, options);
			var expectedText = JsonSerializer.Serialize(expectedObject, options);
			Assert.AreEqual(expectedText, actualText);
		}
	}
}
