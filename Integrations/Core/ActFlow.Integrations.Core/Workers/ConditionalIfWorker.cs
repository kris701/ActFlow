using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.Core.Workers
{
	public class ConditionalIfWorker : BaseWorker<ConditionalIfActivity>
	{
		public ConditionalIfWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ConditionalIfActivity act, WorkflowState state, CancellationToken token)
		{
			state.AppendToLog($"Evaluated: '{act.LeftCondition}' '{Enum.GetName(act.Comparer)}' '{act.RightCondition}' = '{act.LeftCondition == act.RightCondition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.LeftCondition == act.RightCondition)
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.LeftCondition != act.RightCondition)
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.LeftCondition.Contains(act.RightCondition))
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new WorkerResult(new EmptyContext(), act.FalseActivityName);
		}
	}
}
