using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Integrations.JSON.Activities
{
	public class ExtractValueFromJSONActivity : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsontext";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		[JsonPropertyName("jsonPath")]
		public string JSONPath { get; set; }

		public ExtractValueFromJSONActivity(string name, string workerId, string text, string jsonPath)
		{
			Name = name;
			WorkerID = workerId;
			Text = text;
			JSONPath = jsonPath;
		}

		public IActivity Clone() => new ExtractValueFromJSONActivity(Name, WorkerID, Text, JSONPath);
	}
}
