using ActFlow.Integrations.OpenWebUISharp.OpenWebUI;
using ActFlow.Integrations.OpenWebUISharp.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.OpenWebUISharp.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExtractDataFromTextLLMWorker("a", new OpenWebUIService("","")) };
			yield return new object[] {
				new ExtractDataFromTextRAGLLMWorker("a", new OpenWebUIService("","")) };
			yield return new object[] {
				new QueryLLMWorker("a", new OpenWebUIService("","")) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
