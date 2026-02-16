using ActChain.Integrations.Core.Workers;
using ActChain.Models.Workers;
using ActChain.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActChain.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ConditionalIfWorker("a"),
				"TestFiles/Workers/ConditionalIfWorker_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserWorker("a", 1000),
				"TestFiles/Workers/ConditionalUserWorker_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextWorker("a"),
				"TestFiles/Workers/CreateContextWorker_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsWorker("a"),
				"TestFiles/Workers/InsertGlobalsWorker_Serialization_Expected.json" };
			yield return new object[] {
				new NoActionWorker("a"),
				"TestFiles/Workers/NoActionWorker_Serialization_Expected.json" };
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
