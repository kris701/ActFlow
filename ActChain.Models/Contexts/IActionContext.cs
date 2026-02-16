using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	public interface IActionContext
	{
		public string GetContent();
		public IActionContext Clone();
		public Dictionary<string, string> GetContextValues();
	}
}
