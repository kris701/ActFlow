using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	[JsonDerivedType(typeof(DictionaryListContext), typeDiscriminator: nameof(DictionaryListContext))]
	public class DictionaryListContext : IActionContext
	{
		public Dictionary<string, List<string>> Values { get; set; } = new Dictionary<string, List<string>>();

		public string GetContent() => JsonSerializer.Serialize(Values);

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(DictionaryListContext));
			newDict.Add("dictlistdata", JsonSerializer.Serialize(JsonSerializer.Serialize(Values)));
			return newDict;
		}

		public IActionContext Clone()
		{
			var newAct = new DictionaryListContext();

			foreach (var key in Values.Keys)
				newAct.Values.Add(key, new List<string>(Values[key]));

			return newAct;
		}

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
