using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class LoadFileWorker : BaseWorker<LoadFileActivity>
	{
		public LoadFileWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(LoadFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var path = Path.Combine(PersistenDirectory, act.Path);
			if (!File.Exists(path))
				throw new Exception("File not found!");
			var txt = File.ReadAllText(path);

			return new WorkerResult(new StringContext()
			{
				Text = txt
			});
		}
	}
}
