using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class NoActivityWorker : BaseWorker<NoActivity>
	{
		public NoActivityWorker()
		{
		}

		public NoActivityWorker(string id) : base(id)
		{
		}

		

		public override async Task<WorkerResult> Execute(NoActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			return new WorkerResult();
		}
	}
}
