namespace ActFlow.Models.Workflows
{
	public class WorkflowLog
	{
		public WorkflowLogTypes LogType { get; set; }
		public string Text { get; set; }

		public WorkflowLog(WorkflowLogTypes logType, string text)
		{
			LogType = logType;
			Text = text;
		}

		public WorkflowLog(string text)
		{
			LogType = WorkflowLogTypes.Info;
			Text = text;
		}

		public WorkflowLog()
		{
			LogType = WorkflowLogTypes.Info;
			Text = "";
		}
	}
}
