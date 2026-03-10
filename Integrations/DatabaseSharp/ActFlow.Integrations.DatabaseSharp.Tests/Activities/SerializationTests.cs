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
				new ExecuteSTPActivity("abc", "a", "sp_a", new Dictionary<string, string>(), 0, new Dictionary<string, string>()) };
			yield return new object[] {
				new InsertWorkflowFromDatabaseActivity("abc", "a", "sp_a", new Dictionary<string, string>(), false) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
