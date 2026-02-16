using ActChain.Models.Attributes;
using ActChain.Models.Scripts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Models.Workers
{
	public interface IWorker
	{
		[JsonPropertyName("id")]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string ID { get; set; }

		public Task<WorkerResult> ExecuteActionAsync(dynamic act, WorkflowState state, CancellationToken token);
	}
}
