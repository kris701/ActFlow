using ActFlow.Integrations.Time.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.Time.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new DelayWorker("a") };
			yield return new object[] {
				new CronWaitWorker("a") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
