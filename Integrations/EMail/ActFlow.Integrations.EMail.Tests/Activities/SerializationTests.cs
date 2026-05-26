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
				new ReplyToEmailActivity(){ Name = "abc", WorkerID = "ww", Answer = new EmptyContext(), ToMessageID = "123" } };
			yield return new object[] {
				new SendEmailActivity(){ Name = "abc", WorkerID = "w", Answer = new EmptyContext() } };
			yield return new object[] {
				new WaitForEmailActivity(){ Name = "abc", WorkerID = "w", ConversationID = "123", RecieverEmail = "abc@mail.com", SenderEmail = "sss@email.com" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IActivity input) => SerializationHelpers.SerializeTest(input);
	}
}
