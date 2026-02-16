using ActChain.Integrations.Core.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.Core.Workers
{
	public class NoActionWorker : BaseWorker<NoActivity>
	{
		public NoActionWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(NoActivity act, ActScriptState state, CancellationToken token)
		{
			return new WorkerResult();
		}
	}
}
