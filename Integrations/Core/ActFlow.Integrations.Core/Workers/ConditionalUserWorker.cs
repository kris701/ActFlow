using ActFlow.Integrations.Core.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.Core.Workers
{
	public class ConditionalUserWorker : BaseWorker<ConditionalUserActivity>
	{
		[Required]
		public int WaitDelayMs { get; set; }

		[JsonConstructor]
		public ConditionalUserWorker(int waitDelayMs)
		{
			WaitDelayMs = waitDelayMs;
		}

		public override async Task<WorkerResult> Execute(ConditionalUserActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act.HumanInput == null)
			{
				state.Status = WorkflowStatuses.AwaitingHumanInput;
				await state.Update();
			}

			while (act.HumanInput == null)
			{
				await Task.Delay(WaitDelayMs);
				if (token.IsCancellationRequested)
					return new WorkerResult();
			}

			state.AppendToLog($"Evaluated: '{act.HumanInput}' '{Enum.GetName(act.Comparer)}' '{act.Condition}' = '{act.HumanInput == act.Condition}'");

			switch (act.Comparer)
			{
				case ConditionalComparerTypes.Equal:
					if (act.HumanInput == act.Condition)
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				case ConditionalComparerTypes.NotEqual:
					if (act.HumanInput != act.Condition)
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				case ConditionalComparerTypes.Contains:
					if (act.HumanInput.Contains(act.Condition))
						return new WorkerResult(new EmptyContext(), act.TrueActivityName);
					break;
				default:
					throw new Exception($"Unknown comparison method: '{act.Comparer}'");
			}

			return new WorkerResult(new EmptyContext(), act.FalseActivityName);
		}
	}
}
