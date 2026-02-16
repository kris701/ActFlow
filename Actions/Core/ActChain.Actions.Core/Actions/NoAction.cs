using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	public class NoAction : IAIAction
	{
		public string Name { get; set; } = "noaction";
		public string ExecutorID { get; set; } = "default";

		public NoAction(string name, string executorId)
		{
			Name = name;
			ExecutorID = executorId;
		}

		public IAIAction Clone() => new NoAction(Name, ExecutorID);
	}
}
