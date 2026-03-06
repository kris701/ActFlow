using ActFlow.Integrations.Core.Activities;
using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class InsertGlobalsWorkerTests
	{
		public static IEnumerable<object[]> InputModels()
		{
			yield return new object[] {
				new InsertGlobalsActivity("", "", new Dictionary<string, string>(){ { "abc","123" } }) };
			yield return new object[] {
				new InsertGlobalsActivity("", "", new Dictionary<string, string>(){ { "abc","123" }, { "aaaaa", "1" } }) };
		}

		[TestMethod]
		[DynamicData(nameof(InputModels))]
		public async Task Can_Execute(InsertGlobalsActivity activity)
		{
			// ARRANGE
			var worker = new InsertGlobalsWorker("");
			var state = new WorkflowState();

			// ACT
			var result = await worker.Execute(activity, state, new CancellationToken(), "");

			// ASSERT
			foreach (var global in activity.Arguments.Keys)
			{
				Assert.IsTrue(state.ContextStore.ContainsKey(global));
				Assert.AreEqual(state.ContextStore[global], activity.Arguments[global]);
			}
		}
	}
}
