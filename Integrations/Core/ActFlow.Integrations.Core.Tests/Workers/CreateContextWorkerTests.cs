using ActFlow.Integrations.Core.Activities;
using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class CreateContextWorkerTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new DictionaryContext(){ Values = new Dictionary<string, string>() } };
			yield return new object[] {
				new DictionaryListContext(){ Values = new Dictionary<string, List<string>>() } };
			yield return new object[] {
				new EmptyContext() };
			yield return new object[] {
				new ListContext(){ Values = new List<string>() } };
			yield return new object[] {
				new StringContext(){ Text = "abc" } };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public async Task Can_Execute(IContext context)
		{
			// ARRANGE
			var worker = new CreateContextWorker("");
			var activity = new CreateContextActivity("act", "", context);

			// ACT
			var result = await worker.Execute(activity, new WorkflowState(), new CancellationToken(), "");

			// ASSERT
			Assert.IsExactInstanceOfType(result.Context, context.GetType());
		}
	}
}
