using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Actions
{
	[JsonDerivedType(typeof(InsertItemToDatabaseAction), typeDiscriminator: nameof(InsertItemToDatabaseAction))]
	public class InsertItemToDatabaseAction : IAIAction
	{
		public string Name { get; set; } = "insertitemintodatabase";
		public string ExecutorID { get; set; } = "default";
		public string TargetSTP { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public InsertItemToDatabaseAction(string name, string executorId, string targetSTP, Dictionary<string, string> arguments)
		{
			Name = name;
			ExecutorID = executorId;
			TargetSTP = targetSTP;
			Arguments = arguments;
		}

		public IAIAction Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertItemToDatabaseAction(Name, ExecutorID, TargetSTP, arguments);
		}
	}
}
