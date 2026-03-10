using ActFlow.Models.Activities;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.JSON.Activities
{
	public class ExtractValuesFromJSONActivity : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsontext";
		public string WorkerID { get; set; } = "default";

		public string JSON { get; set; }
		[JsonPropertyName("jsonPaths")]
		public Dictionary<string, string> JSONPaths { get; set; }

		public ExtractValuesFromJSONActivity(string name, string workerId, string json, Dictionary<string, string> jsonPaths)
		{
			Name = name;
			WorkerID = workerId;
			JSON = json;
			JSONPaths = jsonPaths;
		}

		public IActivity Clone()
		{
			var jsonPaths = new Dictionary<string, string>();
			foreach (var key in JSONPaths.Keys)
				jsonPaths.Add(key, JSONPaths[key]);
			return new ExtractValuesFromJSONActivity(Name, WorkerID, JSON, jsonPaths);
		}
	}
}
