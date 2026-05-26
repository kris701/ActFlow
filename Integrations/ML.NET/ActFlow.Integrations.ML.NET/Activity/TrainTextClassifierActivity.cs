using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.ML.NET.Activity
{
	public class TrainTextClassifierActivity : IActivity
	{
		public string Name { get; set; } = "trainatextclassifier";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string ModelName { get; set; }
		[Required]
		public string Data { get; set; }

		public IActivity Clone() => new TrainTextClassifierActivity() {
			Name = Name,
			WorkerID = WorkerID,
			ModelName = ModelName,
			Data = Data
		};
	}
}
