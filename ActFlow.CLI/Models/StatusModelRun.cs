namespace ActFlow.CLI.Models
{
	public class StatusModelRun
	{
		public Guid ID { get; set; }
		public string Name { get; set; }
		public TimeSpan Runtime { get; set; }
	}
}
