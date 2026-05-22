using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class ListFilesWorker : BaseWorker<ListFilesActivity>
	{
		public ListFilesWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ListFilesActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var path = Path.Combine(PersistenDirectory, act.Path);
			var dirInfo = new DirectoryInfo(path);
			var files = dirInfo.GetFiles();
			return new WorkerResult(new ListContext()
			{
				Values = files.Select(x => x.Name).ToList()
			});
		}
	}
}
