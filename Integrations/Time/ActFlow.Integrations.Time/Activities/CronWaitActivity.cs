using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Time.Activities
{
	public class CronWaitActivity : IActivity
	{
		public string Name { get; set; }
		public string WorkerID { get; set; }
		public string CronExpression { get; set; }

		public CronWaitActivity(string name, string workerID, string cronExpression)
		{
			Name = name;
			WorkerID = workerID;
			CronExpression = cronExpression;
		}

		public IActivity Clone() => new CronWaitActivity(Name, WorkerID, CronExpression);
	}
}
