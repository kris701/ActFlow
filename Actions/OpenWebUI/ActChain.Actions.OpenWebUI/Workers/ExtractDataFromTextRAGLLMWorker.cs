using ActChain.Actions.OpenWebUI.Activities;
using ActChain.Actions.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Workers
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
