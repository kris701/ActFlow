using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Actions
{
	public class TrainTextClassifierAction : IActivity
	{
		public string Name { get; set; } = "trainatextclassifier";
		public string WorkerID { get; set; } = "default";
		public string ModelName { get; set; }
		public string Data { get; set; }

		public TrainTextClassifierAction(string name, string executorId, string modelName, string data)
		{
			Name = name;
			WorkerID = executorId;
			ModelName = modelName;
			Data = data;
		}

		public IActivity Clone() => new TrainTextClassifierAction(Name, WorkerID, ModelName, Data);
	}
}
