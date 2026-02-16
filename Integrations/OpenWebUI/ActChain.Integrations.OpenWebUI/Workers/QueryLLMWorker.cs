using ActChain.Integrations.OpenWebUI.Activities;
using ActChain.Integrations.OpenWebUI.OpenWebUI;
using ActChain.Models.Contexts;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.OpenWebUI.Workers
{
	public class QueryLLMWorker : BaseWorker<QueryLLMActivity>
	{
		public IOpenWebUIService OpenWebUIService { get; set; }
		public QueryLLMWorker(string iD, IOpenWebUIService openWebUIService) : base(iD)
		{
			OpenWebUIService = openWebUIService;
		}

		public override async Task<WorkerResult> Execute(QueryLLMActivity act, WorkflowState state, CancellationToken token)
		{
			var result = await OpenWebUIService.Query(act.Prompt, act.Model);
			return new WorkerResult(new StringContext() { Text = result });
		}
	}
}
