using ActFlow.Integrations.OpenWebUI.Activities;
using ActFlow.Models.Activities;
using ActFlow.TestTools;

namespace ActFlow.Integrations.OpenWebUISharp.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExtractDataFromTextLLMActivity("abc", "a", "", "", "") };
			yield return new object[] {
				new ExtractDataFromTextRAGLLMActivity("abc", "a", "", new List<string>(), "", "") };
			yield return new object[] {
				new QueryLLMActivity("abc", "a", "", "") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
