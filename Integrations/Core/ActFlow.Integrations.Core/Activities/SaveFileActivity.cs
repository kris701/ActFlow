using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;

namespace ActFlow.Integrations.Core.Activities
{
	public class SaveFileActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		public string Data { get; set; } = "";
		public string Path { get; set; } = "file.txt";
		public FileDirectories Directory { get; set; } = FileDirectories.Temporary;

		public SaveFileActivity(string name, string workerId, string data, string path, FileDirectories directory)
		{
			Name = name;
			WorkerID = workerId;
			Data = data;
			Path = path;
			Directory = directory;
		}

		public IActivity Clone() => new SaveFileActivity(Name, WorkerID, Data, Path, Directory);
	}
}
