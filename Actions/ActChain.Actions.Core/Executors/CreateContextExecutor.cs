using ActChain.Actions.Core.Actions;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	[JsonDerivedType(typeof(CreateContextExecutor), typeDiscriminator: nameof(CreateContextExecutor))]
	public class CreateContextExecutor : BaseActionExecutor<CreateContextAction>
	{
		public CreateContextExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(CreateContextAction act, ActScriptState state)
		{
			return new ExecutorResult(act.Context);
		}
	}
}
