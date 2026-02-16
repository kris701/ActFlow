using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	[JsonDerivedType(typeof(ExtractDataFromTextLLMAction), typeDiscriminator: nameof(ExtractDataFromTextLLMAction))]
	public class ExtractDataFromTextLLMAction : IAIAction
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string ExecutorID { get; set; } = "default";

		public string Text { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ExtractDataFromTextLLMAction(string name, string executorId, string text, string prompt, string model)
		{
			Name = name;
			ExecutorID = executorId;
			Text = text;
			Prompt = prompt;
			Model = model;
		}

		public IAIAction Clone() => new ExtractDataFromTextLLMAction(Name, ExecutorID, Text, Prompt, Model);
	}
}
