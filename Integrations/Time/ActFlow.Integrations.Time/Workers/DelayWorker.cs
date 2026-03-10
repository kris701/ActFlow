using ActFlow.Integrations.Time.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Time.Workers
{
	public class DelayWorker : BaseWorker<DelayActivity>
	{
		public DelayWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(DelayActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var timespan = TimeSpan.Parse(act.Delay);
			await Task.Delay(timespan, token);
			return new WorkerResult();
		}
	}
}
