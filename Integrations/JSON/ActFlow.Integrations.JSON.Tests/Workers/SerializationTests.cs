using ActFlow.Integrations.JSON.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.JSON.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExtractValuesFromJSONWorker("a") };
			yield return new object[] {
				new ExtractValuesFromJSONFileWorker("a") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
