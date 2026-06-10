using ActFlow.Models.Contexts;

namespace ActFlow.CLI.Models
{
	public class UpdateHumanInput
	{
		public Guid ID { get; set; }
		public IContext Input { get; set; }
	}
}
