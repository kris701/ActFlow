using ActFlow.Integrations.JSON.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.JSON.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExtractValueFromJSONActivity("abc", "a", "abc", "$0") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
