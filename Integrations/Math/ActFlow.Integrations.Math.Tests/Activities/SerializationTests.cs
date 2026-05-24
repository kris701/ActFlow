using ActFlow.Integrations.Math.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.Math.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new EvaluateActivity(){
					WorkerID = "a",
					Name = "n",
					Left = "1",
					Op = EvaluateActivity.OperatorTypes.Mul,
					Right = "10"
				} };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
