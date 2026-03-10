using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Time.Activities
{
	public class DelayActivity : IActivity
	{
		public string Name { get; set; }
		public string WorkerID { get; set; }
		public string Delay { get; set; }

		public DelayActivity(string name, string workerID, string delay)
		{
			Name = name;
			WorkerID = workerID;
			Delay = delay;
		}

		public IActivity Clone() => new DelayActivity(Name, WorkerID, Delay);
	}
}
