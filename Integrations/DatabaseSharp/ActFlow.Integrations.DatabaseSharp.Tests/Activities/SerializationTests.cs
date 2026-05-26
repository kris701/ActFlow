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
				new ExecuteSTPActivity(){ 
					Name = "abc", 
					WorkerID = "a", 
					TargetSTP = "stp_val", 
					Arguments = new Dictionary<string, string>(){
						{ "abc","111" }
					},
					ResultMap = new Dictionary<string, string>(){
						{ "bbb", "asdfadf" }
					},
					ResultTable = 3}};
			yield return new object[] {
				new InsertWorkflowFromDatabaseActivity(){
					Name = "abc",
					WorkerID = "b",
					TargetSTP = "stp_val",
					Arguments = new Dictionary<string, string>()
					{
						{ "abc", "lll1" }
					},
					HasInserted = false
				} };
			yield return new object[] {
				new ExecuteSQLActivity(){ 
					Name = "aaa",
					WorkerID = "wo",
					SQL = "SELECT ABC",
					ResultMap = new Dictionary<string, string>(){
						{ "bbb", "asdfadf" }
					},
					ResultTable = 3
				} };
			yield return new object[] {
				new ExecuteSQLToFileActivity(){ WorkerID = "abc", Name = "a", Path = "path", Directory = Models.Contexts.FileDirectories.Persistent, ResultTable = 0, SQL = "SELECT" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
