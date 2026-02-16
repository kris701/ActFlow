using ActChain.Models.Actions;

namespace ActChain.Models.Scripts
{
	public class ActScript
	{
		public string Name { get; set; }
		public Dictionary<string, string> Globals { get; set; } = new Dictionary<string, string>();
		public List<IAIAction> Stages { get; set; } = new List<IAIAction>();


	}
}
