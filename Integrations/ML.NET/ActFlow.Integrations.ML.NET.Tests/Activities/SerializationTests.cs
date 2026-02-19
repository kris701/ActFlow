using ActFlow.Integrations.ML.NET.Activity;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.ML.NET.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ClassifyTextActivity("abc", "a", "text", "mod") };
			yield return new object[] {
				new TrainTextClassifierActivity("abc", "a", "mod", "a;1") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
