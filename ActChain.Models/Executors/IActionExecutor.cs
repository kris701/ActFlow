using ActChain.Models.Attributes;
using ActChain.Models.Scripts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Models.Executors
{
	public interface IActionExecutor
	{
		[JsonPropertyName("id")]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string ID { get; set; }

		public Task<ExecutorResult> ExecuteActionAsync(dynamic act, ActScriptState state, CancellationToken token);
	}
}
