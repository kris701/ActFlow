using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	[JsonDerivedType(typeof(ExtractDataFromTextRAGLLMAction), typeDiscriminator: nameof(ExtractDataFromTextRAGLLMAction))]
	public class ExtractDataFromTextRAGLLMAction : IAIAction
	{
		public string Name { get; set; } = "extractdatawithllmrag";
		public string ExecutorID { get; set; } = "default";

		public string Text { get; set; }
		public List<string> Collections { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ExtractDataFromTextRAGLLMAction(string name, string executorId, string text, List<string> collections, string prompt, string model)
		{
			Name = name;
			ExecutorID = executorId;
			Text = text;
			Collections = collections;
			Prompt = prompt;
			Model = model;
		}

		public IAIAction Clone() => new ExtractDataFromTextRAGLLMAction(Name, ExecutorID, Text, new List<string>(Collections), Prompt, Model);
	}
}
