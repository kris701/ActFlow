using ActChain.Actions.Core.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	[JsonDerivedType(typeof(ConditionalIfExecutor), typeDiscriminator: nameof(ConditionalIfExecutor))]
	public class ConditionalIfExecutor : BaseActionExecutor<ConditionalIfAction>
	{
		public ConditionalIfExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ConditionalIfAction act, ActScriptState state)
		{
			state.AppendToLog($"Evaluated: '{act.LeftCondition}' '{act.Comparer.ToLower().Trim()}' '{act.RightCondition}' = '{act.LeftCondition == act.RightCondition}'");

			switch (act.Comparer.ToLower().Trim())
			{
				case "==":
					if (act.LeftCondition == act.RightCondition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case "!=":
					if (act.LeftCondition != act.RightCondition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case "contains":
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
