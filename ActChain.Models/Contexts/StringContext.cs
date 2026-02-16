using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	public class StringContext : IContext
	{
		public string Text { get; set; } = "Empty";

		public string GetContent() => Text;
		public Dictionary<string, string> GetContextValues() => new Dictionary<string, string>() { { "$type", nameof(StringContext) }, { "text", Text } };
		public IContext Clone() => new StringContext() { Text = Text };

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
