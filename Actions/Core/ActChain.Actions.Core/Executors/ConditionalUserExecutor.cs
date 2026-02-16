using ActChain.Actions.Core.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	public class ConditionalUserExecutor : BaseActionExecutor<ConditionalUserAction>
	{
		public int WaitDelayMs { get; set; }

		public ConditionalUserExecutor(string iD, int waitDelayMs) : base(iD)
		{
			WaitDelayMs = waitDelayMs;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ConditionalUserAction act, ActScriptState state, CancellationToken token)
		{
			while (act.UserInput == "")
			{
				await Task.Delay(WaitDelayMs);
				if (token.IsCancellationRequested)
					return new ExecutorResult();
			}

			state.AppendToLog($"Evaluated: '{act.UserInput}' '{Enum.GetName(act.Comparer)}' '{act.Condition}' = '{act.UserInput == act.Condition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.UserInput == act.Condition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.UserInput != act.Condition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.UserInput.Contains(act.Condition))
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new ExecutorResult(new EmptyContext(), act.FalseActionName);
		}
	}
}
