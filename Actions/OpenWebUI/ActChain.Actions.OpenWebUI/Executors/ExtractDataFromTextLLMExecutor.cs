using ActChain.Actions.OpenWebUI.Actions;
using ActChain.Actions.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Executors
{
	[JsonDerivedType(typeof(ExtractDataFromTextLLMExecutor), typeDiscriminator: nameof(ExtractDataFromTextLLMExecutor))]
	public class ExtractDataFromTextLLMExecutor : BaseActionExecutor<ExtractDataFromTextLLMAction>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }
		public ExtractDataFromTextLLMExecutor(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ExtractDataFromTextLLMAction act, ActScriptState state)
		{
			var sb = new StringBuilder();
			sb.AppendLine(act.Prompt);
			sb.AppendLine();
			sb.AppendLine($"Text: {act.Text}");

			var result = await OpenWebUIService.QueryToJsonObject<Dictionary<string, string>>(sb.ToString(), act.Model);
			return new ExecutorResult(new DictionaryContext() { Values = result });
		}
	}
}
