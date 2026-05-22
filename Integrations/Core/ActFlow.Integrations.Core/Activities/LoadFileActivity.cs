using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;

namespace ActFlow.Integrations.Core.Activities
{
	public class LoadFileActivity : IActivity
	{
		public string Name { get; set; } = "loadfile";
		public string WorkerID { get; set; } = "default";
		public string Path { get; set; } = "file.txt";
		public FileDirectories Directory { get; set; } = FileDirectories.Temporary;

		public LoadFileActivity(string name, string workerId, string path, FileDirectories directory)
		{
			Name = name;
			WorkerID = workerId;
			Path = path;
			Directory = directory;
		}

		public IActivity Clone() => new LoadFileActivity(Name, WorkerID, Path, Directory);
	}
}
