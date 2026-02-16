using ActChain.Models.Actions;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	[JsonDerivedType(typeof(QueryLLMAction), typeDiscriminator: nameof(QueryLLMAction))]
	public class QueryLLMAction : IAIAction
	{
		public string Name { get; set; } = "queryllm";
		public string ExecutorID { get; set; } = "default";

		public string Prompt { get; set; }
		public string Model { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public QueryLLMAction(string name, string executorId, string prompt, string model)
		{
			Name = name;
			ExecutorID = executorId;
			Prompt = prompt;
			Model = model;
		}

		public IAIAction Clone() => new QueryLLMAction(Name, ExecutorID, Prompt, Model);
	}
}
