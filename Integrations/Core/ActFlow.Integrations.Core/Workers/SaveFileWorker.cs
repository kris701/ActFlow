using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class SaveFileWorker : BaseWorker<SaveFileActivity>
	{
		public SaveFileWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(SaveFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var path = Path.Combine(PersistenDirectory, act.Path);
			File.WriteAllText(path, act.Data);
			return new WorkerResult();
		}
	}
}
