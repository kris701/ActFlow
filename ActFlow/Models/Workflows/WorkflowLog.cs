namespace ActFlow.Models.Workflows
{
	public class WorkflowLog
	{
		public WorkflowLogTypes LogType { get; set; }
		public string Text { get; set; }
		public DateTime Timestamp { get; set; }

		public WorkflowLog(WorkflowLogTypes logType, string text)
		{
			LogType = logType;
			Text = text;
			Timestamp = DateTime.UtcNow;
		}

		public WorkflowLog(string text)
		{
			LogType = WorkflowLogTypes.Info;
			Text = text;
			Timestamp = DateTime.UtcNow;
		}

		public WorkflowLog()
		{
			LogType = WorkflowLogTypes.Info;
			Text = "";
			Timestamp = DateTime.UtcNow;
		}
	}
}
