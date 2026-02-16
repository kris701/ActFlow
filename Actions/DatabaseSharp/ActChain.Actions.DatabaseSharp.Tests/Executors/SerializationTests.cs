using ActChain.Actions.DatabaseSharp.Executors;
using ActChain.Models.Executors;
using ActChain.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.DatabaseSharp.Tests.Executors
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new FetchItemsFromDatabaseExecutor("a", ""),
				"TestFiles/Executors/FetchItemsFromDatabaseExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new InsertChainFromDatabaseExecutor("a", ""),
				"TestFiles/Executors/InsertChainFromDatabaseExecutor_Serialization_Expected.json" };
			yield return new object[] {
				new InsertItemToDatabaseExecutor("a", ""),
				"TestFiles/Executors/InsertItemToDatabaseExecutor_Serialization_Expected.json" };
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
