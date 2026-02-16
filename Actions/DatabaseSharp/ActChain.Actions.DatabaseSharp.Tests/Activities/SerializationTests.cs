using ActChain.Actions.DatabaseSharp.Activities;
using ActChain.Models.Activities;
using ActChain.Models.Contexts;
using ActChain.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Actions.DatabaseSharp.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new FetchItemsFromDatabaseAction("abc", "a", "sp_a", new Dictionary<string, string>(), "FetchItemsFromDatabaseAction"),
				"TestFiles/Activities/FetchItemsFromDatabaseAction_Serialization_Expected.json" };
			yield return new object[] {
				new InsertChainFromDatabaseAction("abc", "a", "sp_a", new Dictionary<string, string>()),
				"TestFiles/Activities/InsertChainFromDatabaseAction_Serialization_Expected.json" };
			yield return new object[] {
				new InsertItemToDatabaseAction("abc", "a", "sp_a", new Dictionary<string, string>()),
				"TestFiles/Activities/InsertItemToDatabaseAction_Serialization_Expected.json" };
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
