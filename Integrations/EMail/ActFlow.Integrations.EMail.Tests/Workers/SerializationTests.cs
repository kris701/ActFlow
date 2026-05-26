using ActFlow.Integrations.EMail.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.EMail.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ReplyToEmailWorker(new EMail.OutlookMailService("",0,"","","")) };
			yield return new object[] {
				new SendEmailWorker(new EMail.OutlookMailService("",0,"","","")) };
			yield return new object[] {
				new WaitForEmailWorker(1000, new EMail.OutlookMailService("",0,"","","")) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
