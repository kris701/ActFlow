using ActFlow.Integrations.Javascript.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using Jint;

namespace ActFlow.Integrations.Javascript.Workers
{
	public class ExecuteJavascriptWorker : BaseWorker<ExecuteJavascriptActivity>
	{
		public long MemoryLimit { get; set; }
		public int MaxStatements { get; set; }

		public ExecuteJavascriptWorker(string iD, long memoryLimit, int maxStatements) : base(iD)
		{
			MemoryLimit = memoryLimit;
			MaxStatements = maxStatements;
		}

		public override async Task<WorkerResult> Execute(ExecuteJavascriptActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var workerResult = new WorkerResult();

			var engine = new Engine(o =>
			{
				o.LimitMemory(MemoryLimit);
				o.MaxStatements(MaxStatements);
				o.CancellationToken(token);
			})
				.SetValue("log", new Action<string>(state.AppendToLog))
				.SetValue("result", workerResult);
			await engine.ExecuteAsync(act.Code);

			return workerResult;
		}
	}
}
