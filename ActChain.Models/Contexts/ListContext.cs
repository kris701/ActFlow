using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	public class ListContext : IContext
	{
		public List<string> Values { get; set; } = new List<string>();

		public string GetContent()
		{
			var sb = new StringBuilder();

			foreach (var value in Values)
				sb.AppendLine($"\"{value}\"");

			return sb.ToString();
		}

		public Dictionary<string, string> GetContextValues() => new Dictionary<string, string>() { { "$type", nameof(ListContext) } };

		public IContext Clone() => new ListContext() { Values = new List<string>(Values) };

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
