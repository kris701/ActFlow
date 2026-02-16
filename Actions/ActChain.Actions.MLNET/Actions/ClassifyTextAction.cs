using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.MLNET.Actions
{
	[JsonDerivedType(typeof(ClassifyTextAction), typeDiscriminator: nameof(ClassifyTextAction))]
	public class ClassifyTextAction : IAIAction
	{
		public string Name { get; set; } = "extractdatawithllm";
		public string ExecutorID { get; set; } = "default";

		public string Text { get; set; }
		public string Model { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ClassifyTextAction(string name, string executorID, string text, string model)
		{
			Name = name;
			ExecutorID = executorID;
			Text = text;
			Model = model;
		}

		public IAIAction Clone() => new ClassifyTextAction(Name, ExecutorID, Text, Model);
	}
}
