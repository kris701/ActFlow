using ActChain.Actions.Core.Activities;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
{
	public class CreateContextExecutor : BaseWorker<CreateContextAction>
	{
		public CreateContextExecutor(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(CreateContextAction act, ActScriptState state, CancellationToken token)
		{
			return new WorkerResult(act.Context);
		}
	}
}
