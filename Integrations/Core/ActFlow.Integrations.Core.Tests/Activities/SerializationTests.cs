using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow.Integrations.Core.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new NoActivity("abc", "a"),
				"TestFiles/Activities/NoActivity_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalIfActivity("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/Activities/ConditionalIfActivity_Serialization_Expected.json" };
			yield return new object[] {
				new ConditionalUserActivity("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc"),
				"TestFiles/Activities/ConditionalUserActivity_Serialization_Expected.json" };
			yield return new object[] {
				new CreateContextActivity("abc", "a", new EmptyContext()),
				"TestFiles/Activities/CreateContextActivity_Serialization_Expected.json" };
			yield return new object[] {
				new InsertGlobalsActivity("abc", "a", new Dictionary<string, string>() { { "a", "asdasdad" } }),
				"TestFiles/Activities/InsertGlobalsActivity_Serialization_Expected.json" };
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
