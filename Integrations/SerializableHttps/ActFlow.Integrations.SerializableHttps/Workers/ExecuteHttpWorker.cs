using ActFlow.Integrations.SerializableHttps.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using SerializableHttps;

namespace ActFlow.Integrations.SerializableHttps.Workers
{
	public class ExecuteHttpWorker : BaseWorker<ExecuteHttpActivity>
	{
		public override async Task<WorkerResult> Execute(ExecuteHttpActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var client = new SerializableHttpsClient();
			foreach (var key in act.Headers.Keys)
				client.Headers.Add(key, act.Headers[key]);

			var result = "";
			switch (act.Type)
			{
				case HttpTypes.POST:
					result = await client.PostAsync<string, string>(act.Content, act.Route);
					break;
				case HttpTypes.GET:
					result = await client.GetAsync<string, string>(act.Content, act.Route);
					break;
				case HttpTypes.PATCH:
					result = await client.PatchAsync<string, string>(act.Content, act.Route);
					break;
				case HttpTypes.DELETE:
					result = await client.DeleteAsync<string, string>(act.Content, act.Route);
					break;
			}
			if (result == null)
				result = "";

			return new WorkerResult(new StringContext()
			{
				Text = result
			});
		}
	}
}
