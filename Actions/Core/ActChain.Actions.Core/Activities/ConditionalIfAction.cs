using ActChain.Models.Activities;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class ConditionalIfAction : IActivity
	{
		public string Name { get; set; } = "conditionalif";
		public string WorkerID { get; set; } = "default";
		public string LeftCondition { get; set; }
		public string RightCondition { get; set; }
		public ConditionalComparerTypes Comparer { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActionName { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActionName { get; set; }

		public ConditionalIfAction(string name, string executorId, string leftCondition, string rightCondition, ConditionalComparerTypes comparer, string trueActionName, string falseActionName)
		{
			Name = name;
			WorkerID = executorId;
			LeftCondition = leftCondition;
			RightCondition = rightCondition;
			Comparer = comparer;
			TrueActionName = trueActionName;
			FalseActionName = falseActionName;
		}

		public IActivity Clone() => new ConditionalIfAction(Name, WorkerID, LeftCondition, RightCondition, Comparer, TrueActionName, FalseActionName);
	}
}
