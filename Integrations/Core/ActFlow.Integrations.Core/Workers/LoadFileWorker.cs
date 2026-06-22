using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class LoadFileWorker : BaseWorker<LoadFileActivity>
	{
		public override async Task<WorkerResult> Execute(LoadFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var txt = await LoadFile(act.Path, act.Directory, tmpDirectory, state, token);
			return new WorkerResult(new StringContext()
			{
				Text = txt
			});
		}
	}
}
