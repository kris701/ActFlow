using ActChain.Models.Actions;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Models.Executors
{
	public abstract class BaseActionExecutor<T> : IActionExecutor where T : IAIAction
	{
		[JsonPropertyName("id")]
		public string ID { get; set; }

		protected BaseActionExecutor(string iD)
		{
			ID = iD;
		}


		public abstract Task<ExecutorResult> ExecuteActionAsync(T act, ActScriptState state, CancellationToken token);

		public async Task<ExecutorResult> ExecuteActionAsync(dynamic act, ActScriptState state, CancellationToken token)
		{
			if (act is T actualAct)
				return await ExecuteActionAsync(actualAct, state, token);
			throw new Exception("Invalid action input to executor!");
		}
	}
}
