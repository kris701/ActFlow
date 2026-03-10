using ActFlow.Integrations.Time.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using Cronos;

namespace ActFlow.Integrations.Time.Workers
{
	public class CronWaitWorker : BaseWorker<CronWaitActivity>
	{
		public CronWaitWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(CronWaitActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			CronExpression expression = CronExpression.Parse(act.CronExpression);
			var next = expression.GetNextOccurrence(DateTime.UtcNow);
			if (next == null)
				return new WorkerResult();
			var waitTime = next - DateTime.UtcNow;
			if (waitTime == null)
				return new WorkerResult();
			await Task.Delay((TimeSpan)waitTime, token);
			return new WorkerResult();
		}
	}
}
