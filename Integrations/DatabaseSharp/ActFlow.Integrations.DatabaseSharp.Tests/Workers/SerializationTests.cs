using ActFlow.Integrations.DatabaseSharp.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.DatabaseSharp.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ExecuteSTPWorker("") };
			yield return new object[] {
				new InsertWorkflowFromDatabaseWorker("") };
			yield return new object[] {
				new ExecuteSQLWorker("") };
			yield return new object[] {
				new ExecuteSQLToFileWorker("") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
