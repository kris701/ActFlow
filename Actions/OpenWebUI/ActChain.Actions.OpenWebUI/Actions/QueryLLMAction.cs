using ActChain.Models.Activities;
using System.Text.Json.Serialization;

namespace ActChain.Actions.OpenWebUI.Actions
{
	public class QueryLLMAction : IActivity
	{
		public string Name { get; set; } = "queryllm";
		public string WorkerID { get; set; } = "default";

		public string Prompt { get; set; }
		public string Model { get; set; }

		public QueryLLMAction(string name, string executorId, string prompt, string model)
		{
			Name = name;
			WorkerID = executorId;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new QueryLLMAction(Name, WorkerID, Prompt, Model);
	}
}
