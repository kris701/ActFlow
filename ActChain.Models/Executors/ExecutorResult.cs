using ActChain.Models.Contexts;

namespace ActChain.Models.Executors
{
	public class ExecutorResult
	{
		public IActionContext Context { get; set; }
		public string TargetAction { get; set; }

		public ExecutorResult()
		{
			Context = new EmptyContext();
			TargetAction = "";
		}

		public ExecutorResult(IActionContext context)
		{
			Context = context;
			TargetAction = "";
		}

		public ExecutorResult(IActionContext context, string targetAction)
		{
			Context = context;
			TargetAction = targetAction;
		}
	}
}
