using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Javascript.Activities
{
	public class ExecuteJavascriptActivity : IActivity
	{
		public string WorkerID { get; set; }
		public string Name { get; set; }

		public string Code { get; set; }

		public ExecuteJavascriptActivity(string workerID, string name, string code)
		{
			WorkerID = workerID;
			Name = name;
			Code = code;
		}

		public IActivity Clone() => new ExecuteJavascriptActivity(WorkerID, Name, Code);
	}
}
