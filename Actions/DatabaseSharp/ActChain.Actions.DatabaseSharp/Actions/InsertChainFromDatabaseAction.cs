using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Actions
{
	public class InsertChainFromDatabaseAction : IAIAction
	{
		public string Name { get; set; } = "insertchain";
		public string ExecutorID { get; set; } = "default";

		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }

		public InsertChainFromDatabaseAction(string name, string executorID, string targetSTP, Dictionary<string, string> arguments)
		{
			Name = name;
			ExecutorID = executorID;
			TargetSTP = targetSTP;
			Arguments = arguments;
		}

		public IAIAction Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertChainFromDatabaseAction(Name, ExecutorID, TargetSTP, arguments);
		}
	}
}
