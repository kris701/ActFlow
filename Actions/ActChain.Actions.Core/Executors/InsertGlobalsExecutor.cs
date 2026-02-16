using ActChain.Actions.Core.Actions;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Executors
{
	[JsonDerivedType(typeof(InsertGlobalsExecutor), typeDiscriminator: nameof(InsertGlobalsExecutor))]
	public class InsertGlobalsExecutor : BaseActionExecutor<InsertGlobalsAction>
	{
		public InsertGlobalsExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(InsertGlobalsAction act, ActScriptState state)
		{
			state.AddContexts(act.Arguments);
			return new ExecutorResult();
		}
	}
}
