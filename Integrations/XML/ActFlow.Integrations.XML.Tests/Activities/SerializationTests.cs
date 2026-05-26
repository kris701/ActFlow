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
				new ExtractValuesFromXMLActivity(){ Name = "abc", WorkerID = "a", XML = "ssxs", XPaths = new Dictionary<string, string>() } };
			yield return new object[] {
				new ExtractValuesFromXMLFileActivity(){ Name = "aaa", WorkerID = "b", XPaths = new Dictionary<string, string>(), Directory = Models.Contexts.FileDirectories.Temporary, Path = "bbb.xml" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
