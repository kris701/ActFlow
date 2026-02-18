using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class NoActionWorker : BaseWorker<NoActivity>
	{
		public NoActionWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(NoActivity act, WorkflowState state, CancellationToken token)
		{
			return new WorkerResult();
		}
	}
}
