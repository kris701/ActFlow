using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	public class ExtractDataFromTextLLMAction : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }

		public ExtractDataFromTextLLMAction(string name, string executorId, string text, string prompt, string model)
		{
			Name = name;
			WorkerID = executorId;
			Text = text;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new ExtractDataFromTextLLMAction(Name, WorkerID, Text, Prompt, Model);
	}
}
