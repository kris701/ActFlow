using ActFlow.Models.Activities;

namespace ActFlow.Integrations.OpenWebUI.Activities
{
	public class QueryLLMActivity : IActivity
	{
		public string Name { get; set; } = "queryllm";
		public string WorkerID { get; set; } = "default";

		public string Prompt { get; set; }
		public string Model { get; set; }

		public QueryLLMActivity(string name, string workerId, string prompt, string model)
		{
			Name = name;
			WorkerID = workerId;
			Prompt = prompt;
			Model = model;
		}

		public IActivity Clone() => new QueryLLMActivity(Name, WorkerID, Prompt, Model);
	}
}
