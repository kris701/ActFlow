using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Actions
{
	[JsonDerivedType(typeof(TrainTextClassifierAction), typeDiscriminator: nameof(TrainTextClassifierAction))]
	public class TrainTextClassifierAction : IAIAction
	{
		public string Name { get; set; } = "trainatextclassifier";
		public string ExecutorID { get; set; } = "default";
		public string ModelName { get; set; }
		public string Data { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public TrainTextClassifierAction(string name, string executorId, string modelName, string data)
		{
			Name = name;
			ExecutorID = executorId;
			ModelName = modelName;
			Data = data;
		}

		public IAIAction Clone() => new TrainTextClassifierAction(Name, ExecutorID, ModelName, Data);
	}
}
