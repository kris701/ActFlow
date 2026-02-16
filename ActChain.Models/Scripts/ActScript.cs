using ActChain.Models.Activities;

namespace ActChain.Models.Scripts
{
	public class ActScript
	{
		public string Name { get; set; }
		public Dictionary<string, string> Globals { get; set; } = new Dictionary<string, string>();
		public List<IActivity> Stages { get; set; } = new List<IActivity>();


	}
}
