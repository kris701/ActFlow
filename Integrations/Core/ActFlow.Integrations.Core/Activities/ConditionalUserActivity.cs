using ActFlow.Models.Activities;
using ActFlow.Models.Attributes;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class ConditionalUserActivity : IActivity, IHumanInput
	{
		public string Name { get; set; } = "waitforuserinput";
		public string WorkerID { get; set; } = "default";
		public string? HumanInput { get; set; }
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

		public void Apply(IContext context)
		{
			switch (context)
			{
				case StringContext c:
					HumanInput = c.Text;
					break;
				default:
					throw new Exception("Invalid context type!");
			}
		}

		public IActivity Clone() => new ConditionalUserActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			HumanInput = HumanInput,
			Condition = Condition,
			Comparer = Comparer,
			TrueActivityName = TrueActivityName,
			FalseActivityName = FalseActivityName
		};
	}
}
