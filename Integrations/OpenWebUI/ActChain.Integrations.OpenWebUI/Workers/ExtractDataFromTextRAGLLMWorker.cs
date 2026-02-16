using ActChain.Integrations.OpenWebUI.Activities;
using ActChain.Integrations.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using System.Text;

namespace ActChain.Integrations.OpenWebUI.Workers
{
	public class ExtractDataFromTextRAGLLMWorker : BaseWorker<ExtractDataFromTextRAGLLMActivity>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }

		public ExtractDataFromTextRAGLLMWorker(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<WorkerResult> Execute(ExtractDataFromTextRAGLLMActivity act, ActScriptState state, CancellationToken token)
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
