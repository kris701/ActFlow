using ActChain.Actions.Core.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	public class ConditionalIfExecutor : BaseActionExecutor<ConditionalIfAction>
	{
		public ConditionalIfExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ConditionalIfAction act, ActScriptState state, CancellationToken token)
		{
			state.AppendToLog($"Evaluated: '{act.LeftCondition}' '{Enum.GetName(act.Comparer)}' '{act.RightCondition}' = '{act.LeftCondition == act.RightCondition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.LeftCondition == act.RightCondition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.LeftCondition != act.RightCondition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.LeftCondition.Contains(act.RightCondition))
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new ExecutorResult(new EmptyContext(), act.FalseActionName);
		}
	}
}
