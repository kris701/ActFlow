using ActFlow.Integrations.OpenWebUI.Activities;
using ActFlow.Integrations.OpenWebUI.OpenWebUI;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text;

namespace ActFlow.Integrations.OpenWebUI.Workers
{
	public class ExtractDataFromTextRAGLLMWorker : BaseWorker<ExtractDataFromTextRAGLLMActivity>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }

		public ExtractDataFromTextRAGLLMWorker(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<WorkerResult> Execute(ExtractDataFromTextRAGLLMActivity act, WorkflowState state, CancellationToken token)
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
