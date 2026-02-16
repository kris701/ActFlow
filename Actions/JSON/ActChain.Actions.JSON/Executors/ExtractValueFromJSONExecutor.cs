using ActChain.Actions.JSON.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using Json.Path;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ActChain.Actions.JSON.Executors
{
	public class ExtractValueFromJSONExecutor : BaseActionExecutor<ExtractValueFromJSONAction>
	{
		public ExtractValueFromJSONExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ExtractValueFromJSONAction act, ActScriptState state, CancellationToken token)
		{
			var path = JsonPath.Parse(act.JSONPath.ToLower());
			var instance = JsonNode.Parse(act.Text.ToLower());
			var results = path.Evaluate(instance);
			if (results.Matches.Count == 0)
				throw new Exception("Could not find any match for the JSON Path!");
			if (results.Matches[0].Value == null)
				throw new Exception("Could not find any match for the JSON Path!");
			var value = results.Matches[0].Value.ToString().ToLower();

			return new ExecutorResult(new StringContext() { Text = value });
		}
	}
}
