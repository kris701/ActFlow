using System.Text.Json;

namespace ActFlow.Models.Contexts
{
	public class ListDictionaryContext : IContext
	{
		public List<Dictionary<string, string>> Values { get; set; } = new List<Dictionary<string, string>>();

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(ListDictionaryContext));
			var index = 0;
			foreach (var item in Values)
			{
				foreach (var key in item.Keys)
					newDict.Add($"{index}.{key}", item[key]);
				index++;
			}
			return newDict;
		}

		public IContext Clone()
		{
			var newAct = new ListDictionaryContext();

			foreach (var item in Values)
			{
				var newDict = new Dictionary<string, string>();
				foreach (var key in item.Keys)
					newDict.Add(key, item[key]);
				newAct.Values.Add(newDict);
			}

			return newAct;
		}

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
