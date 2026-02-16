using ActChain.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActChain.Models.Actions
{
	public interface IAIAction
	{
		public IAIAction Clone();
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string ExecutorID { get; set; }
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string Name { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }
	}
}
