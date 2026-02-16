using ActChain.Actions.Core.Activities;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
{
	public class NoActionExecutor : BaseWorker<NoAction>
	{
		public NoActionExecutor(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(NoAction act, ActScriptState state, CancellationToken token)
		{
			return new WorkerResult();
		}
	}
}
