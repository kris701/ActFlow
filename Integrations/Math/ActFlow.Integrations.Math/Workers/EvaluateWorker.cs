using ActFlow.Integrations.Math.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Math.Workers
{
	public class EvaluateWorker : BaseWorker<EvaluateActivity>
	{
		public EvaluateWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(EvaluateActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			double leftVal = 0;
			if (!Double.TryParse(act.Left, System.Globalization.CultureInfo.InvariantCulture, out leftVal))
				throw new Exception($"Could not convert the value '{act.Left}' to a valid double!");
			double rightVal = 0;
			if (!Double.TryParse(act.Right, System.Globalization.CultureInfo.InvariantCulture, out rightVal))
				throw new Exception($"Could not convert the value '{act.Left}' to a valid double!");

			switch (act.Op)
			{
				case EvaluateActivity.OperatorTypes.Add:
					return new WorkerResult(new StringContext() { Text = $"{leftVal + rightVal}" });
				case EvaluateActivity.OperatorTypes.Sub:
					return new WorkerResult(new StringContext() { Text = $"{leftVal - rightVal}" });
				case EvaluateActivity.OperatorTypes.Div:
					return new WorkerResult(new StringContext() { Text = $"{leftVal / rightVal}" });
				case EvaluateActivity.OperatorTypes.Mul:
					return new WorkerResult(new StringContext() { Text = $"{leftVal * rightVal}" });
				default:
					throw new Exception("Unknown operator?");
			}
		}
	}
}
