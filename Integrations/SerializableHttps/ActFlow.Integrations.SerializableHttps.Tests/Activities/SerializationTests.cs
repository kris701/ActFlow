using ActFlow.Integrations.SerializableHttps.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.SerializableHttps.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExecuteHttpActivity() {
					WorkerID = "a",
					Name = "b",
					Content ="{ 'id':'a' }",
					Headers = new Dictionary<string, string>(),
					Route = "/abc",
					Type = HttpTypes.PATCH
				} };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
