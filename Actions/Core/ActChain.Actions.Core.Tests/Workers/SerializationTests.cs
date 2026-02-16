using ActChain.Actions.Core.Activities;
using ActChain.Actions.Core.Workers;
using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.Core.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ConditionalIfExecutor("a"),
				"TestFiles/Workers/ConditionalIfExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserExecutor("a", 1000),
				"TestFiles/Workers/ConditionalUserExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextExecutor("a"),
				"TestFiles/Workers/CreateContextExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsExecutor("a"),
				"TestFiles/Workers/InsertGlobalsExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new NoActionExecutor("a"),
				"TestFiles/Workers/NoActionExecutor_Serialization_Expected.json" };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input, string expectedFile)
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
			var expectedObject = JsonSerializer.Deserialize<IWorker>(expectedFileText, options);
			var expectedText = JsonSerializer.Serialize(expectedObject, options);
			Assert.AreEqual(expectedText, actualText);
		}
	}
}
