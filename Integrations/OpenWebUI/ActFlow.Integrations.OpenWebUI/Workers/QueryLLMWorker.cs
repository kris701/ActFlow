using ActFlow.Integrations.OpenWebUI.Activities;
using ActFlow.Integrations.OpenWebUI.OpenWebUI;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.OpenWebUI.Workers
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
