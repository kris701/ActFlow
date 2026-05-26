using ActFlow.Integrations.OpenWebUISharp.Activities;
using ActFlow.Integrations.OpenWebUISharp.OpenWebUI;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.OpenWebUISharp.Workers
{
	public class QueryLLMWorker : BaseWorker<QueryLLMActivity>
	{
		[Required]
		public IOpenWebUIService OpenWebUIService { get; set; }

		public override async Task<WorkerResult> Execute(QueryLLMActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var result = await OpenWebUIService.Query(act.Prompt, act.Model);
			return new WorkerResult(new StringContext() { Text = result });
		}
	}
}
