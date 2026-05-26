using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.OpenWebUISharp.Activities
{
	public class ExtractDataFromTextRAGLLMActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllmrag";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Text { get; set; }
		[Required]
		public List<string> Collections { get; set; }
		[Required]
		public string Prompt { get; set; }
		[Required]
		public string Model { get; set; }

		public IActivity Clone() => new ExtractDataFromTextRAGLLMActivity() { 
			Name = Name,
			WorkerID = WorkerID,
			Text = Text,
			Collections = new List<string>(Collections),
			Prompt = Prompt,
			Model = Model
		};
	}
}
