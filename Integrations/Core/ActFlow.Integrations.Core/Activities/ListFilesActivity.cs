using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Core.Activities
{
	public class ListFilesActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		public string Path { get; set; } = "";

		public ListFilesActivity(string name, string workerId, string path)
		{
			Name = name;
			WorkerID = workerId;
			Path = path;
		}

		public IActivity Clone() => new ListFilesActivity(Name, WorkerID, Path);
	}
}
