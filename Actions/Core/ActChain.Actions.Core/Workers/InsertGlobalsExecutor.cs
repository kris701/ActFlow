using ActChain.Actions.Core.Activities;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
{
	public class InsertGlobalsExecutor : BaseWorker<InsertGlobalsAction>
	{
		public InsertGlobalsExecutor(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(InsertGlobalsAction act, ActScriptState state, CancellationToken token)
		{
			state.AddContexts(act.Arguments);
			return new WorkerResult();
		}
	}
}
