using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class InsertGlobalsWorker : BaseWorker<InsertGlobalsActivity>
	{
		public InsertGlobalsWorker()
		{
		}

		public InsertGlobalsWorker(string id) : base(id)
		{
		}

		

		public override async Task<WorkerResult> Execute(InsertGlobalsActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			state.AddContexts(act.Arguments);
			return new WorkerResult();
		}
	}
}
