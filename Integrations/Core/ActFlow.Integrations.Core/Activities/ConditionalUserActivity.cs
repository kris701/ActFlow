using ActFlow.Models.Activities;
using ActFlow.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class ConditionalUserActivity : IActivity, IUpdatableWorkflowActivity
	{
		public string Name { get; set; } = "waitforuserinput";
		public string WorkerID { get; set; } = "default";
		[Required]
		public string UserInput { get; set; }
		[Required]
		public string Condition { get; set; }
		[Required]
		public ConditionalComparerTypes Comparer { get; set; }
		[Required]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActivityName { get; set; }
		[Required]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActivityName { get; set; }

		public IActivity Clone() => new ConditionalUserActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			UserInput = UserInput,
			Condition = Condition,
			Comparer = Comparer,
			TrueActivityName = TrueActivityName,
			FalseActivityName = FalseActivityName
		};
	}
}
