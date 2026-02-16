using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Activities
{
	public class ExtractDataFromTextLLMActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }

		public ExtractDataFromTextLLMActivity(string name, string workerId, string text, string prompt, string model)
		{
			Name = name;
			WorkerID = workerId;
			Text = text;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new ExtractDataFromTextLLMActivity(Name, WorkerID, Text, Prompt, Model);
	}
}
