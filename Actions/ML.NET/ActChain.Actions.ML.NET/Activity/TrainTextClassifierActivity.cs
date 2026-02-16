using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Actions
{
	public class TrainTextClassifierActivity : IActivity
	{
		public string Name { get; set; } = "trainatextclassifier";
		public string WorkerID { get; set; } = "default";
		public string ModelName { get; set; }
		public string Data { get; set; }

		public TrainTextClassifierActivity(string name, string workerId, string modelName, string data)
		{
			Name = name;
			WorkerID = workerId;
			ModelName = modelName;
			Data = data;
		}

		public IActivity Clone() => new TrainTextClassifierActivity(Name, WorkerID, ModelName, Data);
	}
}
