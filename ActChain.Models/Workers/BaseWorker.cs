using ActChain.Models.Activities;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Models.Workers
{
	public abstract class BaseWorker<T> : IWorker where T : IActivity
	{
		[JsonPropertyName("id")]
		public string ID { get; set; }

		protected BaseWorker(string iD)
		{
			ID = iD;
		}


		public abstract Task<WorkerResult> Execute(T act, ActScriptState state, CancellationToken token);

		public async Task<WorkerResult> ExecuteActionAsync(dynamic act, ActScriptState state, CancellationToken token)
		{
			if (act is T actualAct)
				return await Execute(actualAct, state, token);
			throw new Exception("Invalid action input to executor!");
		}
	}
}
