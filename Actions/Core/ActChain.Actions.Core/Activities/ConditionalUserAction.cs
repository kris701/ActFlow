using ActChain.Models.Activities;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Activities
{
	public class ConditionalUserAction : IActivity, IAwaitInputActivity
	{
		public string Name { get; set; } = "waitforuserinput";
		public string WorkerID { get; set; } = "default";
		public string UserInput { get; set; }
		public string Condition { get; set; }
		public ConditionalComparerTypes Comparer { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActionName { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActionName { get; set; }

		public ConditionalUserAction(string name, string executorId, string userInput, string condition, ConditionalComparerTypes comparer, string trueActionName, string falseActionName)
		{
			Name = name;
			UserInput = userInput;
			Condition = condition;
			Comparer = comparer;
			TrueActionName = trueActionName;
			FalseActionName = falseActionName;
		}

		public IActivity Clone() => new ConditionalUserAction(Name, WorkerID, UserInput, Condition, Comparer, TrueActionName, FalseActionName);
	}
}
