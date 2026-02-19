using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Activities;
using ActFlow.Models.Workers;
using ActFlow.TestTools;
using ActFlow.Tools.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ConditionalIfWorker("a") };
			yield return new object[] {
				new ConditionalUserWorker("a", 1000) };
			yield return new object[] {
				new CreateContextWorker("a") };
			yield return new object[] {
				new InsertGlobalsWorker("a") };
			yield return new object[] {
				new NoActivityWorker("a") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
