using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.JSON.Actions
{
	[JsonDerivedType(typeof(ExtractValueFromJSONAction), typeDiscriminator: nameof(ExtractValueFromJSONAction))]
	public class ExtractValueFromJSONAction : IAIAction
	{
		public string Name { get; set; } = "extractasinglevaluefromjsontext";
		public string ExecutorID { get; set; } = "default";

		public string Text { get; set; }
		[JsonPropertyName("jsonPath")]
		public string JSONPath { get; set; }

		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ExtractValueFromJSONAction(string name, string executorId, string text, string jsonPath)
		{
			Name = name;
			ExecutorID = executorId;
			Text = text;
			JSONPath = jsonPath;
		}

		public IAIAction Clone() => new ExtractValueFromJSONAction(Name, ExecutorID, Text, JSONPath);
	}
}
