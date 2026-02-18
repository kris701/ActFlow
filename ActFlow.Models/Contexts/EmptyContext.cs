using System.Text.Json;

namespace ActFlow.Models.Contexts
{
	public class EmptyContext : IContext
	{
		public string GetContent() => "Empty";
		public Dictionary<string, string> GetContextValues() => new Dictionary<string, string>() { { "$type", nameof(EmptyContext) } };
		public IContext Clone() => new EmptyContext();
		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
