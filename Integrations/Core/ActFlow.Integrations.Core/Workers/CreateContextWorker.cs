using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class CreateContextWorker : BaseWorker<CreateContextActivity>
	{
		public CreateContextWorker()
		{
		}

		public CreateContextWorker(string id) : base(id)
		{
		}

		

		public override async Task<WorkerResult> Execute(CreateContextActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			return new WorkerResult(act.Context);
		}
	}
}
