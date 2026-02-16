using ActChain.Actions.Core.Activities;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
{
	public class ConditionalIfExecutor : BaseWorker<ConditionalIfAction>
	{
		public ConditionalIfExecutor(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ConditionalIfAction act, ActScriptState state, CancellationToken token)
		{
			state.AppendToLog($"Evaluated: '{act.LeftCondition}' '{Enum.GetName(act.Comparer)}' '{act.RightCondition}' = '{act.LeftCondition == act.RightCondition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.LeftCondition == act.RightCondition)
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.LeftCondition != act.RightCondition)
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.LeftCondition.Contains(act.RightCondition))
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new WorkerResult(new EmptyContext(), act.FalseActionName);
		}
	}
}
