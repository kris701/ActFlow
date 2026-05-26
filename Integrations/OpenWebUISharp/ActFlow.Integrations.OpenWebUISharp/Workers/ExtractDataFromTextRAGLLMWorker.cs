using ActFlow.Integrations.OpenWebUISharp.Activities;
using ActFlow.Integrations.OpenWebUISharp.OpenWebUI;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActFlow.Integrations.OpenWebUISharp.Workers
{
	public class ExtractDataFromTextRAGLLMWorker : BaseWorker<ExtractDataFromTextRAGLLMActivity>
	{
		[Required]
		public IOpenWebUIService OpenWebUIService { get; set; }

		public override async Task<WorkerResult> Execute(ExtractDataFromTextRAGLLMActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var sb = new StringBuilder();
			sb.AppendLine(act.Prompt);
			sb.AppendLine();
			sb.AppendLine($"Text: {act.Text}");

			var result = await OpenWebUIService.QueryToJsonObject<Dictionary<string, string>>(sb.ToString(), act.Model, act.Collections.Select(x => new Guid(x)).ToList());
			return new WorkerResult(new DictionaryContext() { Values = result });
		}
	}
}
