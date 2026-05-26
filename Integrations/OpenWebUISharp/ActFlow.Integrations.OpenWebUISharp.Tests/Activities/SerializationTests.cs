using ActFlow.Integrations.OpenWebUISharp.Activities;
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
				new ExtractDataFromTextLLMActivity(){ Name = "abc", WorkerID = "a", Text = "abc", Model = "mod", Prompt = "aaa" } };
			yield return new object[] {
				new ExtractDataFromTextRAGLLMActivity(){ Name = "abc", WorkerID = "a", Text = "ta", Model = "ddd", Prompt = "aaa", Collections = new List<string>(){ "set" } } };
			yield return new object[] {
				new QueryLLMActivity(){ Name = "abc", WorkerID = "a", Prompt = "abc", Model = "mod" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
