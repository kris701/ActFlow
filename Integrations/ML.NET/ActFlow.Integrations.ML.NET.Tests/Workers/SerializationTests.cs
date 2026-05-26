using ActFlow.Integrations.ML.NET.Worker;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.ML.NET.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ClassifyTextWorker() };
			yield return new object[] {
				new TrainTextClassifierWorker() };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
