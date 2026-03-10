using ActFlow.Integrations.JSON.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using Json.Path;
using System.Text.Json.Nodes;

namespace ActFlow.Integrations.JSON.Workers
{
	public class ExtractValuesFromJSONWorker : BaseWorker<ExtractValuesFromJSONActivity>
	{
		public ExtractValuesFromJSONWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ExtractValuesFromJSONActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var resultDict = new Dictionary<string, string>();
			foreach (var key in act.JSONPaths.Keys)
			{
				var path = JsonPath.Parse(act.JSONPaths[key].ToLower());
				var instance = JsonNode.Parse(act.JSON.ToLower());
				var results = path.Evaluate(instance);
				if (results.Matches.Count == 0)
					throw new Exception("Could not find any match for the JSON Path!");
				if (results.Matches[0].Value == null)
					throw new Exception("Could not find any match for the JSON Path!");
				var value = results.Matches[0].Value.ToString().ToLower();
				resultDict.Add(key, value);
			}

			return new WorkerResult(new DictionaryContext() { Values = resultDict });
		}
	}
}
