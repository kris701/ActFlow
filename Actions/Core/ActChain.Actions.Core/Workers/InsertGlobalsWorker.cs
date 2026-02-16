using ActChain.Actions.Core.Activities;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
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
