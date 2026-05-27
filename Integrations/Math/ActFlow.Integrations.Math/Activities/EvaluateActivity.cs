using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Math.Activities
{
	public class EvaluateActivity : IActivity
	{
		public enum OperatorTypes { Add, Sub, Div, Mul }

		public string WorkerID { get; set; } = "default";
		public string Name { get; set; } = "evaluateexp";

		[Required]
		public string Left { get; set; }
		[Required]
		public OperatorTypes Op { get; set; }
		[Required]
		public string Right { get; set; }
	}
}
