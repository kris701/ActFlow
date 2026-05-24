using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Math.Activities
{
	public class EvaluateActivity : IActivity
	{
		public enum OperatorTypes { Add, Sub, Div, Mul }

		public string WorkerID { get; set; }
		public string Name { get; set; }

		public string Left { get; set; }
		public OperatorTypes Op { get; set; }
		public string Right { get; set; }


		public IActivity Clone() => new EvaluateActivity()
		{
			WorkerID = WorkerID,
			Name = Name,
			Left = Left,
			Op = Op,
			Right = Right
		};
	}
}
