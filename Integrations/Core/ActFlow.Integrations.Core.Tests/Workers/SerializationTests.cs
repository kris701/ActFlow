using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Workers;
using ActFlow.TestTools;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class SerializationTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new ConditionalIfWorker() };
			yield return new object[] {
				new ConditionalUserWorker(1000) };
			yield return new object[] {
				new CreateContextWorker() };
			yield return new object[] {
				new InsertGlobalsWorker() };
			yield return new object[] {
				new NoActivityWorker() };
			yield return new object[] {
				new LoadFileWorker() };
			yield return new object[] {
				new SaveFileWorker() };
			yield return new object[] {
				new ListFilesWorker() };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public void Can_Serialize(IWorker input) => SerializationHelpers.SerializeTest(input);
	}
}
