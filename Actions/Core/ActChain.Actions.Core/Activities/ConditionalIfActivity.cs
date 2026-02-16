using ActChain.Models.Activities;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class ConditionalIfActivity : IActivity
	{
		public string Name { get; set; } = "conditionalif";
		public string WorkerID { get; set; } = "default";
		public string LeftCondition { get; set; }
		public string RightCondition { get; set; }
		public ConditionalComparerTypes Comparer { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActivityName { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActivityName { get; set; }

		public ConditionalIfActivity(string name, string workerId, string leftCondition, string rightCondition, ConditionalComparerTypes comparer, string trueActivityName, string falseActivityName)
		{
			Name = name;
			WorkerID = workerId;
			LeftCondition = leftCondition;
			RightCondition = rightCondition;
			Comparer = comparer;
			TrueActivityName = trueActivityName;
			FalseActivityName = falseActivityName;
		}

		public IActivity Clone() => new ConditionalIfActivity(Name, WorkerID, LeftCondition, RightCondition, Comparer, TrueActivityName, FalseActivityName);
	}
}
