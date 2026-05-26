using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.ML.NET.Activity
{
	public class ClassifyTextActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Text { get; set; }
		[Required] 
		public string Model { get; set; }

		public IActivity Clone() => new ClassifyTextActivity() { 
			Name = Name,
			WorkerID = WorkerID,
			Text = Text,
			Model = Model
		};
	}
}
