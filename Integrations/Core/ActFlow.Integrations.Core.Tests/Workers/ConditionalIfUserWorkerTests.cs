using ActFlow.Integrations.Core.Activities;
using ActFlow.Integrations.Core.Workers;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Tests.Workers
{
	[TestClass]
	public class ConditionalIfUserWorkerTests
	{
		[TestMethod]
		[DataRow("abc", "abc", ConditionalComparerTypes.Equal, "act2")]
		[DataRow("abc", "abc", ConditionalComparerTypes.NotEqual, "act3")]
		[DataRow("abc", "abc", ConditionalComparerTypes.Contains, "act2")]
		[DataRow("abc", "123", ConditionalComparerTypes.NotEqual, "act2")]
		[DataRow("abc", "123", ConditionalComparerTypes.Equal, "act3")]
		public async Task Can_Execute(string user, string right, ConditionalComparerTypes compare, string expected)
		{
			// ARRANGE
			var worker = new ConditionalUserWorker("", 10);
			var activity = new ConditionalUserActivity("act", "", "", right, compare, "act2", "act3");

			// ACT
			Task.Run(async () =>
			{
				await Task.Delay(100);
				activity.UserInput = user;
			});

			var result = await worker.Execute(activity, new WorkflowState(), new CancellationToken(), "");

			// ASSERT
			Assert.IsExactInstanceOfType<EmptyContext>(result.Context);
			Assert.AreEqual(expected, result.TargetActivity);
		}
	}
}
