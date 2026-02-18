using ActFlow.Models.Activities;

namespace ActFlow.Integrations.ML.NET.Activity
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
