using ActChain.Integrations.OpenWebUI.Activities;
using ActChain.Integrations.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using System.Text;

namespace ActChain.Integrations.OpenWebUI.Workers
{
	public class ExtractDataFromTextLLMWorker : BaseWorker<ExtractDataFromTextLLMActivity>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }
		public ExtractDataFromTextLLMWorker(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<WorkerResult> Execute(ExtractDataFromTextLLMActivity act, WorkflowState state, CancellationToken token)
		{
			var sb = new StringBuilder();
			sb.AppendLine(act.Prompt);
			sb.AppendLine();
			sb.AppendLine($"Text: {act.Text}");

			var result = await OpenWebUIService.QueryToJsonObject<Dictionary<string, string>>(sb.ToString(), act.Model);
			return new WorkerResult(new DictionaryContext() { Values = result });
		}
	}
}
