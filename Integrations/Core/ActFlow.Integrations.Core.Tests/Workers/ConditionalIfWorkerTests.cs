using ActFlow.Integrations.Core.Activities;
using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class ConditionalIfWorkerTests
	{
		[TestMethod]
		[DataRow("abc", "abc", ConditionalComparerTypes.Equal, "act2")]
		[DataRow("abc", "abc", ConditionalComparerTypes.NotEqual, "act3")]
		[DataRow("abc", "abc", ConditionalComparerTypes.Contains, "act2")]
		[DataRow("abc", "123", ConditionalComparerTypes.NotEqual, "act2")]
		[DataRow("abc", "123", ConditionalComparerTypes.Equal, "act3")]
		public async Task Can_Execute(string left, string right, ConditionalComparerTypes compare, string expected)
		{
			// ARRANGE
			var worker = new ConditionalIfWorker("");
			var activity = new ConditionalIfActivity("act", "", left, right, compare, "act2", "act3");

			// ACT
			var result = await worker.Execute(activity, new WorkflowState(), new CancellationToken(), "");

			// ASSERT
			Assert.IsExactInstanceOfType<EmptyContext>(result.Context);
			Assert.AreEqual(expected, result.TargetActivity);
		}
	}
}
