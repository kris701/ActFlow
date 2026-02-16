using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Actions
{
	public class ClassifyTextAction : IActivity
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string WorkerID { get; set; } = "default";

		public string Text { get; set; }
		public string Model { get; set; }

		public ClassifyTextAction(string name, string executorID, string text, string model)
		{
			Name = name;
			WorkerID = executorID;
			Text = text;
			Model = model;
		}

		public IActivity Clone() => new ClassifyTextAction(Name, WorkerID, Text, Model);
	}
}
