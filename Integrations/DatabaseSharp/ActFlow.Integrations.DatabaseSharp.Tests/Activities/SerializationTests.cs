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
			yield return new object[] {
				new ExecuteSQLActivity("abc", "a", "sp_a", 0, new Dictionary<string, string>()) };
			yield return new object[] {
				new ExecuteSQLToFileActivity(){ WorkerID = "abc", Name = "a", Path = "path", Directory = Models.Contexts.FileDirectories.Persistent, ResultTable = 0, SQL = "SELECT" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
