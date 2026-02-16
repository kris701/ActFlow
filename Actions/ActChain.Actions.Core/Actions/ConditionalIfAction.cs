using ActChain.Models.Actions;
using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	[JsonDerivedType(typeof(ConditionalIfAction), typeDiscriminator: nameof(ConditionalIfAction))]
	public class ConditionalIfAction : IAIAction
	{
		public string Name { get; set; } = "conditionalif";
		public string ExecutorID { get; set; } = "default";
		public string LeftCondition { get; set; }
		public string RightCondition { get; set; }
		public string Comparer { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string TrueActionName { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string FalseActionName { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public ConditionalIfAction(string name, string executorId, string leftCondition, string rightCondition, string comparer, string trueActionName, string falseActionName)
		{
			Name = name;
			ExecutorID = executorId;
			LeftCondition = leftCondition;
			RightCondition = rightCondition;
			Comparer = comparer;
			TrueActionName = trueActionName;
			FalseActionName = falseActionName;
		}

		public IAIAction Clone() => new ConditionalIfAction(Name, ExecutorID, LeftCondition, RightCondition, Comparer, TrueActionName, FalseActionName);
	}
}
