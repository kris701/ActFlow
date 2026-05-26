using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.OpenWebUISharp.Activities
{
	public class ExtractDataFromTextLLMActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Text { get; set; }
		[Required]
		public string Prompt { get; set; }
		[Required]
		public string Model { get; set; }

		public IActivity Clone() => new ExtractDataFromTextLLMActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			Text = Text,
			Prompt = Prompt,
			Model = Model
		};
	}
}
