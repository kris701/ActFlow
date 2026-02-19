using ActFlow.Integrations.EMail.Activities;
using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.TestTools;

namespace ActFlow.Integrations.EMail.Tests.Activities
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ReplyToEmailActivity("abc", "a", "123", new EmptyContext()) };
			yield return new object[] {
				new SendEmailActivity("abc", "a", new EmptyContext()) };
			yield return new object[] {
				new WaitForEmailActivity("abc", "a", "a@a.com", "b@b.com", "123") };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
