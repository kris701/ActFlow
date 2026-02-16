using ActChain.Actions.Core.Actions;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	[JsonDerivedType(typeof(ConditionalUserExecutor), typeDiscriminator: nameof(ConditionalUserExecutor))]
	public class ConditionalUserExecutor : BaseActionExecutor<ConditionalUserAction>
	{
		public int WaitDelayMs { get; set; }

		public ConditionalUserExecutor(string iD, int waitDelayMs) : base(iD)
		{
			WaitDelayMs = waitDelayMs;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ConditionalUserAction act, ActScriptState state)
		{
			while (act.UserInput == "")
			{
				await Task.Delay(WaitDelayMs);
				if (act.Token != null && act.Token.Value.IsCancellationRequested)
					return new ExecutorResult();
			}

			state.AppendToLog($"Evaluated: '{act.UserInput}' '{act.Comparer.ToLower().Trim()}' '{act.Condition}' = '{act.UserInput == act.Condition}'");

			switch (act.Comparer.ToLower().Trim())
			{
				case "==":
					if (act.UserInput == act.Condition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case "!=":
					if (act.UserInput != act.Condition)
						return new ExecutorResult(new EmptyContext(), act.TrueActionName);
					break;
				case "contains":
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
