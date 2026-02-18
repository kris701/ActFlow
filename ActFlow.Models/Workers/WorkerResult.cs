using ActFlow.Models.Contexts;

namespace ActFlow.Models.Workers
{
	public class WorkerResult
	{
		public IContext Context { get; set; }
		public string TargetActivity { get; set; }

		public WorkerResult()
		{
			Context = new EmptyContext();
			TargetActivity = "";
		}

		public WorkerResult(IContext context)
		{
			Context = context;
			TargetActivity = "";
		}

		public WorkerResult(IContext context, string targetAction)
		{
			Context = context;
			TargetActivity = targetAction;
		}
	}
}
