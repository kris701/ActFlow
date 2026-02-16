using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	public class ExtractDataFromTextRAGLLMAction : IActivity
	{
		public string Name { get; set; } = "extractdatawithllmrag";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public List<string> Collections { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }

		public ExtractDataFromTextRAGLLMAction(string name, string executorId, string text, List<string> collections, string prompt, string model)
		{
			Name = name;
			WorkerID = executorId;
			Text = text;
			Collections = collections;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new ExtractDataFromTextRAGLLMAction(Name, WorkerID, Text, new List<string>(Collections), Prompt, Model);
	}
}
