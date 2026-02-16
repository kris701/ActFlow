using ActChain.Integrations.DatabaseSharp.Workers;
using ActChain.Models.Workers;
using ActChain.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Integrations.DatabaseSharp.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new FetchItemsFromDatabaseWorker("a", ""),
				"TestFiles/Workers/FetchItemsFromDatabaseWorker_Serialization_Expected.json" };
			yield return new object[] {
				new InsertChainFromDatabaseWorker("a", ""),
				"TestFiles/Workers/InsertChainFromDatabaseWorker_Serialization_Expected.json" };
			yield return new object[] {
				new InsertItemToDatabaseWorker("a", ""),
				"TestFiles/Workers/InsertItemToDatabaseWorker_Serialization_Expected.json" };
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
