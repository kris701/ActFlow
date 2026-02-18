using ActFlow.Integrations.JSON.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using Json.Path;
using System.Text.Json.Nodes;

namespace ActFlow.Integrations.JSON.Workers
{
	public class ExtractValueFromJSONWorker : BaseWorker<ExtractValueFromJSONActivity>
	{
		public ExtractValueFromJSONWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ExtractValueFromJSONActivity act, WorkflowState state, CancellationToken token)
		{
			var path = JsonPath.Parse(act.JSONPath.ToLower());
			var instance = JsonNode.Parse(act.Text.ToLower());
			var results = path.Evaluate(instance);
			if (results.Matches.Count == 0)
				throw new Exception("Could not find any match for the JSON Path!");
			if (results.Matches[0].Value == null)
				throw new Exception("Could not find any match for the JSON Path!");
			var value = results.Matches[0].Value.ToString().ToLower();

			return new WorkerResult(new StringContext() { Text = value });
		}
	}
}
