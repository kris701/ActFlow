using ActFlow.Integrations.XML.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.XML.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExtractValuesFromXMLActivity("abc", "a", "", new Dictionary<string, string>()) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
