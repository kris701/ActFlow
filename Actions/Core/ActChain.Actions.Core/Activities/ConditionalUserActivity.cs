using ActChain.Models.Activities;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class ConditionalUserActivity : IActivity, IAwaitInputActivity
	{
		public string Name { get; set; } = "waitforuserinput";
		public string WorkerID { get; set; } = "default";
		public string UserInput { get; set; }
		public string Condition { get; set; }
		public ConditionalComparerTypes Comparer { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActivityName { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActivityName { get; set; }

		public ConditionalUserActivity(string name, string workerId, string userInput, string condition, ConditionalComparerTypes comparer, string trueActivityName, string falseActivityName)
		{
			Name = name;
			UserInput = userInput;
			Condition = condition;
			Comparer = comparer;
			TrueActivityName = trueActivityName;
			FalseActivityName = falseActivityName;
		}

		public IActivity Clone() => new ConditionalUserActivity(Name, WorkerID, UserInput, Condition, Comparer, TrueActivityName, FalseActivityName);
	}
}
