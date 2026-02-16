using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	public class EmptyContext : IContext
	{
		public string GetContent() => "Empty";
		public Dictionary<string, string> GetContextValues() => new Dictionary<string, string>() { { "$type", nameof(EmptyContext) } };
		public IContext Clone() => new EmptyContext();
		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
