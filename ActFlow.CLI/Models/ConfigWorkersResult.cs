namespace ActFlow.CLI.Models
{
	public class ConfigWorkersResult
	{
		public string Type { get; set; }
		public string ID { get; set; }

		public ConfigWorkersResult(string type, string iD)
		{
			Type = type;
			ID = iD;
		}
	}
}
