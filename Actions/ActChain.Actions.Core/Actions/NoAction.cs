using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	[JsonDerivedType(typeof(NoAction), typeDiscriminator: nameof(NoAction))]
	public class NoAction : IAIAction
	{
		public string Name { get; set; } = "noaction";
		public string ExecutorID { get; set; } = "default";
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public NoAction(string name, string executorId)
		{
			Name = name;
			ExecutorID = executorId;
		}

		public IAIAction Clone() => new NoAction(Name, ExecutorID);
	}
}
