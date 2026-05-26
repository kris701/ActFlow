using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.TestTools;

namespace ActFlow.Integrations.Core.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new NoActivity() { Name = "abc", WorkerID = "a" } };
			yield return new object[] {
				new ConditionalIfActivity() { Name = "abc", WorkerID = "a", Comparer = ConditionalComparerTypes.Contains, FalseActivityName = "abc", LeftCondition = "a", RightCondition = "q", TrueActivityName = "cba" } };
			yield return new object[] {
				new ConditionalUserActivity(){ Name = "abc", WorkerID = "a", Comparer = ConditionalComparerTypes.Contains, TrueActivityName = "abc", FalseActivityName = "bca", Condition = "a" } };
			yield return new object[] {
				new CreateContextActivity(){ Name = "abc", WorkerID = "a", Context = new StringContext(){ Text = "abc" } } };
			yield return new object[] {
				new InsertGlobalsActivity(){ Name = "abc", WorkerID = "a", Arguments = new Dictionary<string, string>(){ { "value","abc" } } }};
			yield return new object[] {
				new LoadFileActivity(){ Name = "abc", WorkerID = "a", Directory = FileDirectories.Temporary, Path = "abc.txt" }};
			yield return new object[] {
				new SaveFileActivity(){ Name = "abc", WorkerID = "a", Path = "abc.txt", Directory = FileDirectories.Persistent, Data = "abc" }};
			yield return new object[] {
				new ListFilesActivity(){ Name = "abc", WorkerID = "a", Directory = FileDirectories.Temporary, Path = "path" }};
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
