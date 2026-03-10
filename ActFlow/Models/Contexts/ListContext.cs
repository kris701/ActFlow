using System.Text;
using System.Text.Json;

namespace ActFlow.Models.Contexts
{
	public class ListContext : IContext
	{
		public List<string> Values { get; set; } = new List<string>();

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(ListContext));
			var index = 0;
			foreach (var value in Values)
				newDict.Add($"{index++}", value);
			return newDict;
		}

		public IContext Clone() => new ListContext() { Values = new List<string>(Values) };

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
