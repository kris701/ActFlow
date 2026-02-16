using ActChain.Actions.Core.Activities;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Workers
{
	public class ConditionalUserExecutor : BaseWorker<ConditionalUserAction>
	{
		public int WaitDelayMs { get; set; }

		public ConditionalUserExecutor(string iD, int waitDelayMs) : base(iD)
		{
			WaitDelayMs = waitDelayMs;
		}

		public override async Task<WorkerResult> Execute(ConditionalUserAction act, ActScriptState state, CancellationToken token)
		{
			while (act.UserInput == "")
			{
				await Task.Delay(WaitDelayMs);
				if (token.IsCancellationRequested)
					return new WorkerResult();
			}

			state.AppendToLog($"Evaluated: '{act.UserInput}' '{Enum.GetName(act.Comparer)}' '{act.Condition}' = '{act.UserInput == act.Condition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.UserInput == act.Condition)
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.UserInput != act.Condition)
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.UserInput.Contains(act.Condition))
						return new WorkerResult(new EmptyContext(), act.TrueActionName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new WorkerResult(new EmptyContext(), act.FalseActionName);
		}
	}
}
