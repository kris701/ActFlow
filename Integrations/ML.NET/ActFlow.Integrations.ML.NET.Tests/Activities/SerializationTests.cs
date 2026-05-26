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
				new ClassifyTextActivity() { Name = "abc", WorkerID = "w", Model = "mod", Text = "abc" } };
			yield return new object[] {
				new TrainTextClassifierActivity(){ Name = "aaa", WorkerID = "w", Data = "dat", ModelName = "bbb" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
