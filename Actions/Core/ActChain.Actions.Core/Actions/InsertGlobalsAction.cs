using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	public class InsertGlobalsAction : IAIAction
	{
		public string Name { get; set; } = "insertglobal";
		public string ExecutorID { get; set; } = "default";
		public Dictionary<string, string> Arguments { get; set; }

		public InsertGlobalsAction(string name, string executorId, Dictionary<string, string> arguments)
		{
			Name = name;
			ExecutorID = executorId;
			Arguments = arguments;
		}

		public IAIAction Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertGlobalsAction(Name, ExecutorID, arguments);
		}
	}
}
