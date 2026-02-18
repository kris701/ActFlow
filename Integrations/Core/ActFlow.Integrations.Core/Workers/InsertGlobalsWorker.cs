using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class InsertGlobalsWorker : BaseWorker<InsertGlobalsActivity>
	{
		public InsertGlobalsWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(InsertGlobalsActivity act, WorkflowState state, CancellationToken token)
		{
			state.AddContexts(act.Arguments);
			return new WorkerResult();
		}
	}
}
