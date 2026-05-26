using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.OpenWebUISharp.Activities
{
	public class QueryLLMActivity : IActivity
	{
		public string Name { get; set; } = "queryllm";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Prompt { get; set; }
		[Required] 
		public string Model { get; set; }

		public IActivity Clone() => new QueryLLMActivity() { 
			Name = Name,
			WorkerID = WorkerID,
			Prompt = Prompt,
			Model = Model
		};
	}
}
