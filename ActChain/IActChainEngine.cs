using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain
{
	public interface IActChainEngine
	{
		public List<IWorker> Workers { get; }
		public List<ActScriptState> ActiveScripts { get; }
		public int StageLimiter { get; set; }

		public Task<ActScriptState> RunScript(ActScript item);
		public Task<ActScriptState> RunScript(ActScriptState item);
		public Task CancelScript(Guid id);

		public Task CancelAll();

		public Task<ActScriptState> UserInput(Guid chainID, ActScriptState input);
	}
}
