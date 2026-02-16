using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.JSON.Actions
{
	public class ExtractValueFromJSONAction : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsontext";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		[JsonPropertyName("jsonPath")]
		public string JSONPath { get; set; }

		public ExtractValueFromJSONAction(string name, string executorId, string text, string jsonPath)
		{
			Name = name;
			WorkerID = executorId;
			Text = text;
			JSONPath = jsonPath;
		}

		public IActivity Clone() => new ExtractValueFromJSONAction(Name, WorkerID, Text, JSONPath);
	}
}
