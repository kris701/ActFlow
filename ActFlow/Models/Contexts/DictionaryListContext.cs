using System.Text.Json;

namespace ActFlow.Models.Contexts
{
	public class DictionaryListContext : IContext
	{
		public Dictionary<string, List<string>> Values { get; set; } = new Dictionary<string, List<string>>();

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(DictionaryListContext));
			foreach(var key in Values.Keys)
			{
				var index = 0;
				foreach (var value in Values[key])
					newDict.Add($"{key}.{index++}",value);
			}
			return newDict;
		}

		public IContext Clone()
		{
			var newAct = new DictionaryListContext();

			foreach (var key in Values.Keys)
				newAct.Values.Add(key, new List<string>(Values[key]));

			return newAct;
		}

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
