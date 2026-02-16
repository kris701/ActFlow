using ActChain.Actions.OpenWebUI.Actions;
using ActChain.Actions.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Executors
{
	public class QueryLLMExecutor : BaseActionExecutor<QueryLLMAction>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }
		public QueryLLMExecutor(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(QueryLLMAction act, ActScriptState state, CancellationToken token)
		{
			var result = await OpenWebUIService.Query(act.Prompt, act.Model);
			return new ExecutorResult(new StringContext() { Text = result });
		}
	}
}
