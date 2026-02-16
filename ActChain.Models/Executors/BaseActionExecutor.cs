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
	}
}
