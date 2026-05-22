using ActFlow.Models.Activities;

namespace ActFlow.Integrations.Core.Activities
{
	public class SaveFileActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		public string Data { get; set; } = "";
		public string Path { get; set; } = "file.txt";

		public SaveFileActivity(string name, string workerId, string data, string path)
		{
			Name = name;
			WorkerID = workerId;
			Data = data;
			Path = path;
		}

		public IActivity Clone() => new SaveFileActivity(Name, WorkerID, Data, Path);
	}
}
