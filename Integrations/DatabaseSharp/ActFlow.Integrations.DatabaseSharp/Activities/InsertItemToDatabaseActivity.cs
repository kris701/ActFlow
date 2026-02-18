using ActFlow.Models.Activities;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class InsertItemToDatabaseActivity : IActivity
	{
		public string Name { get; set; } = "insertitemintodatabase";
		public string WorkerID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }

		public InsertItemToDatabaseActivity(string name, string workerId, string targetSTP, Dictionary<string, string> arguments)
		{
			Name = name;
			WorkerID = workerId;
			TargetSTP = targetSTP;
			Arguments = arguments;
		}

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertItemToDatabaseActivity(Name, WorkerID, TargetSTP, arguments);
		}
	}
}
