using ActChain.Integrations.Core.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.Core.Workers
{
	public class CreateContextWorker : BaseWorker<CreateContextActivity>
	{
		public CreateContextWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(CreateContextActivity act, WorkflowState state, CancellationToken token)
		{
			return new WorkerResult(act.Context);
		}
	}
}
