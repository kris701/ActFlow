using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	public class DictionaryContext : IActionContext
	{
		public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

		public string GetContent()
		{
			var sb = new StringBuilder();

			foreach (var key in Values.Keys)
				sb.AppendLine($"\"{key}\":\"{Values[key]}\"");

			return sb.ToString();
		}

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(DictionaryContext));
			foreach (var key in Values.Keys)
				newDict.Add(key, Values[key]);
			return newDict;
		}

		public IActionContext Clone()
		{
			var newAct = new DictionaryContext();

			foreach (var key in Values.Keys)
				newAct.Values.Add(key, Values[key]);

			return newAct;
		}

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
