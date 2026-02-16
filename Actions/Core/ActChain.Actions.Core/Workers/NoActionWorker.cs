using ActChain.Actions.Core.Activities;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
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
