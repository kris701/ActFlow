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
				new NoActivity("abc", "a")};
			yield return new object[] {
				new ConditionalIfActivity("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc") };
			yield return new object[] {
				new ConditionalUserActivity("abc", "a", "a", "b", ConditionalComparerTypes.NotEqual, "abc", "abc") };
			yield return new object[] {
				new CreateContextActivity("abc", "a", new EmptyContext()) };
			yield return new object[] {
				new InsertGlobalsActivity("abc", "a", new Dictionary<string, string>() { { "a", "asdasdad" } })};
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
