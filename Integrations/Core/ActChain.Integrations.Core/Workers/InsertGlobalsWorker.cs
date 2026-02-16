using ActChain.Integrations.Core.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.Core.Workers
{
	public class InsertGlobalsWorker : BaseWorker<InsertGlobalsActivity>
	{
		public InsertGlobalsWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(InsertGlobalsActivity act, ActScriptState state, CancellationToken token)
		{
			state.AddContexts(act.Arguments);
			return new WorkerResult();
		}
	}
}
