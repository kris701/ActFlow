using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class CreateContextActivity : IActivity
	{
		public string Name { get; set; } = "createcontext";
		public string WorkerID { get; set; } = "default";
		[Required]
		public IContext Context { get; set; }

		public IActivity Clone() => new CreateContextActivity() {
			Name = Name,
			WorkerID = WorkerID,
			Context = Context
		};
	}
}
