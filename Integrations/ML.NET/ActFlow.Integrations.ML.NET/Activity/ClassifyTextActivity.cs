using ActFlow.Models.Activities;

namespace ActFlow.Integrations.ML.NET.Activity
{
	public class ClassifyTextActivity : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public string Model { get; set; }

		public ClassifyTextActivity(string name, string workerId, string text, string model)
		{
			Name = name;
			WorkerID = workerId;
			Text = text;
			Model = model;
		}

		public IActivity Clone() => new ClassifyTextActivity(Name, WorkerID, Text, Model);
	}
}
