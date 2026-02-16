using ActChain.Models.Actions;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	public class ConditionalUserAction : IAIAction, IAwaitInputAction
	{
		public string Name { get; set; } = "waitforuserinput";
		public string ExecutorID { get; set; } = "default";
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

		public IAIAction Clone() => new ConditionalUserAction(Name, ExecutorID, UserInput, Condition, Comparer, TrueActionName, FalseActionName);
	}
}
