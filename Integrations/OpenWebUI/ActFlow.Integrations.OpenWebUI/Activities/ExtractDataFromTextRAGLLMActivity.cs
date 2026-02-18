using ActFlow.Models.Activities;

namespace ActFlow.Integrations.OpenWebUI.Activities
{
	public class ExtractDataFromTextRAGLLMActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllmrag";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public List<string> Collections { get; set; }
		public string Prompt { get; set; }
		public string Model { get; set; }

		public ExtractDataFromTextRAGLLMActivity(string name, string workerId, string text, List<string> collections, string prompt, string model)
		{
			Name = name;
			WorkerID = workerId;
			Text = text;
			Collections = collections;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new ExtractDataFromTextRAGLLMActivity(Name, WorkerID, Text, new List<string>(Collections), Prompt, Model);
	}
}
