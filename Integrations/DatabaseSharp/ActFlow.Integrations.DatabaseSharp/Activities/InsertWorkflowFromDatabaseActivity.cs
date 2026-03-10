using ActFlow.Models.Activities;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class InsertWorkflowFromDatabaseActivity : IActivity
	{
		public string Name { get; set; } = "insertworkflow";
		public string WorkerID { get; set; } = "default";

		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }

		public bool HasInserted { get; set; } = false;

		public InsertWorkflowFromDatabaseActivity(string name, string workerId, string targetSTP, Dictionary<string, string> arguments, bool hasInserted)
		{
			Name = name;
			WorkerID = workerId;
			TargetSTP = targetSTP;
			Arguments = arguments;
			HasInserted = hasInserted;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertWorkflowFromDatabaseActivity(Name, WorkerID, TargetSTP, arguments, HasInserted);
		}
	}
}
