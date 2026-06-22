using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class SaveFileWorker : BaseWorker<SaveFileActivity>
	{
		public override async Task<WorkerResult> Execute(SaveFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			await SaveFile(act.Path, act.Data, act.Directory, tmpDirectory, state, token);
			return new WorkerResult();
		}
	}
}
