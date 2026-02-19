using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.DatabaseSharp.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new FetchItemsFromDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>(), nameof(FetchItemsFromDatabaseActivity)) };
			yield return new object[] {
				new InsertChainFromDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>()) };
			yield return new object[] {
				new InsertItemToDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>()) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
