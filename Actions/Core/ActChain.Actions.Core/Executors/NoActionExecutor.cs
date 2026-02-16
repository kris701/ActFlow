using ActChain.Actions.Core.Actions;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	[JsonDerivedType(typeof(NoActionExecutor), typeDiscriminator: nameof(NoActionExecutor))]
	public class NoActionExecutor : BaseActionExecutor<NoAction>
	{
		public NoActionExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(NoAction act, ActScriptState state)
		{
			return new ExecutorResult();
		}
	}
}
