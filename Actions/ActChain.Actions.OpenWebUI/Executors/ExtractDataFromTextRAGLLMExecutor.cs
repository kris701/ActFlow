using ActChain.Actions.OpenWebUI.Actions;
using ActChain.Actions.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Executors
{
	[JsonDerivedType(typeof(ExtractDataFromTextRAGLLMExecutor), typeDiscriminator: nameof(ExtractDataFromTextRAGLLMExecutor))]
	public class ExtractDataFromTextRAGLLMExecutor : BaseActionExecutor<ExtractDataFromTextRAGLLMAction>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }

		public ExtractDataFromTextRAGLLMExecutor(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ExtractDataFromTextRAGLLMAction act, ActScriptState state)
		{
			var sb = new StringBuilder();
			sb.AppendLine(act.Prompt);
			sb.AppendLine();
			sb.AppendLine($"Text: {act.Text}");

			var result = await OpenWebUIService.QueryToJsonObject<Dictionary<string, string>>(sb.ToString(), act.Model, act.Collections.Select(x => new Guid(x)).ToList());
			return new ExecutorResult(new DictionaryContext() { Values = result });
		}
	}
}
