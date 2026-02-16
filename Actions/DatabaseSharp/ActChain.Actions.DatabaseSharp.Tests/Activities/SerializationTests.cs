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
				new FetchItemsFromDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>(), "FetchItemsFromDatabaseActivity"),
				"TestFiles/Activities/FetchItemsFromDatabaseActivity_Serialization_Expected.json" };
			yield return new object[] {
				new InsertChainFromDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>()),
				"TestFiles/Activities/InsertChainFromDatabaseActivity_Serialization_Expected.json" };
			yield return new object[] {
				new InsertItemToDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>()),
				"TestFiles/Activities/InsertItemToDatabaseActivity_Serialization_Expected.json" };
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
