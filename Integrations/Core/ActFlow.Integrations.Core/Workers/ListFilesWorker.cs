using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class ListFilesWorker : BaseWorker<ListFilesActivity>
	{
		public override async Task<WorkerResult> Execute(ListFilesActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var files = ListFiles(act.Path, act.Directory, tmpDirectory);
			return new WorkerResult(new ListContext()
			{
				Values = files
			});
		}
	}
}
