using ActFlow.Models.Activities;
using ActFlow.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class ConditionalIfActivity : IActivity
	{
		public string Name { get; set; } = "conditionalif";
		public string WorkerID { get; set; } = "default";
		[Required]
		public string LeftCondition { get; set; }
		[Required]
		public string RightCondition { get; set; }
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

		public IActivity Clone() => new ConditionalIfActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			LeftCondition = LeftCondition,
			RightCondition = RightCondition,
			Comparer = Comparer,
			TrueActivityName = TrueActivityName,
			FalseActivityName = FalseActivityName
		};
	}
}
