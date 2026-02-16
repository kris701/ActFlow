using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain
{
	public interface IActChainEngine
	{
		public List<IWorker> Workers { get; }
		public List<WorkflowState> ActiveWorkflows { get; }
		public int ActivityLimiter { get; set; }

		public Task<WorkflowState> Execute(Workflow workflow);
		public Task<WorkflowState> Execute(WorkflowState state);
		public Task Cancel(Guid id);

		public Task CancelAll();

		public Task<WorkflowState> UserInput(Guid stateId, WorkflowState workflow);
	}
}
