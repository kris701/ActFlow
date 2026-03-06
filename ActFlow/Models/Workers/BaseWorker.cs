using ActFlow.Models.Activities;
using ActFlow.Models.Workflows;
using System.Text.Json.Serialization;

namespace ActFlow.Models.Workers
{
	public abstract class BaseWorker<T> : IWorker where T : IActivity
	{
		[JsonPropertyName("id")]
		public string ID { get; set; }

		[JsonIgnore]
		public string PersistenDirectory { get; set; }

		protected BaseWorker(string iD)
		{
			ID = iD;
		}


		public abstract Task<WorkerResult> Execute(T act, WorkflowState state, CancellationToken token, string tmpDirectory);

		public async Task<WorkerResult> ExecuteActionAsync(dynamic act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act is T actualAct)
				return await Execute(actualAct, state, token, tmpDirectory);
			throw new Exception("Invalid action input to executor!");
		}
	}
}
