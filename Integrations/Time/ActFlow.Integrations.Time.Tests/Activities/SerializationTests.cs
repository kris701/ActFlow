using ActFlow.Integrations.Time.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.Time.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new DelayActivity(){ Name = "abc", WorkerID = "a", Delay = "10" } };
			yield return new object[] {
				new CronWaitActivity(){ Name = "abc", WorkerID = "a", CronExpression = "* * * * *" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
