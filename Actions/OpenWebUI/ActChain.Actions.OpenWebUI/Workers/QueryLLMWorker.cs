using ActChain.Actions.OpenWebUI.Activities;
using ActChain.Actions.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Workers
{
	public class QueryLLMWorker : BaseWorker<QueryLLMActivity>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }
		public QueryLLMWorker(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<WorkerResult> Execute(QueryLLMActivity act, ActScriptState state, CancellationToken token)
		{
			var result = await OpenWebUIService.Query(act.Prompt, act.Model);
			return new WorkerResult(new StringContext() { Text = result });
		}
	}
}
