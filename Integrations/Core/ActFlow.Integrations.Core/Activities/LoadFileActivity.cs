using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Core.Activities
{
	public class LoadFileActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		public string Path { get; set; } = "file.txt";

		public LoadFileActivity(string name, string workerId, string path)
		{
			Name = name;
			WorkerID = workerId;
			Path = path;
		}

		public IActivity Clone() => new LoadFileActivity(Name, WorkerID, Path);
	}
}
