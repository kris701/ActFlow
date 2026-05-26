using ActFlow.Integrations.Javascript.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using Jint;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.Javascript.Workers
{
	public class ExecuteJavascriptWorker : BaseWorker<ExecuteJavascriptActivity>
	{
		[Required]
		public long MemoryLimit { get; set; }
		[Required]
		public int MaxStatements { get; set; }

		[JsonConstructor]
		public ExecuteJavascriptWorker(long memoryLimit, int maxStatements)
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
