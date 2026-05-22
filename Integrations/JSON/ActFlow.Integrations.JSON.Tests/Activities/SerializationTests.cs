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
				new ExtractValuesFromJSONActivity("abc", "a", "abc", new Dictionary<string, string>(){ { "v1", "$0" } }) };
			yield return new object[] {
				new ExtractValuesFromJSONFileActivity("abc", "a", "", Models.Contexts.FileDirectories.Temporary, new Dictionary<string, string>(){ { "v1", "$0" } }) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
