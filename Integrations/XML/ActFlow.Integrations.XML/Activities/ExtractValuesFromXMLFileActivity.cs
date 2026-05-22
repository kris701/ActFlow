using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;

namespace ActFlow.Integrations.XML.Activities
{
	public class ExtractValuesFromXMLFileActivity : IActivity
	{
		public string Name { get; set; } = "extractvaluesfromxml";
		public string WorkerID { get; set; } = "default";
		public string Path { get; set; } = "";
		public FileDirectories Directory { get; set; } = FileDirectories.Temporary;
		public Dictionary<string, string> XPaths { get; set; } = new Dictionary<string, string>();

		public ExtractValuesFromXMLFileActivity(string name, string workerID, string path, FileDirectories directory, Dictionary<string, string> xPaths)
		{
			Name = name;
			WorkerID = workerID;
			Path = path;
			Directory = directory;
			XPaths = xPaths;
		}

		public IActivity Clone()
		{
			var xPaths = new Dictionary<string, string>();
			foreach (var key in XPaths.Keys)
				xPaths.Add(key, XPaths[key]);
			return new ExtractValuesFromXMLFileActivity(Name, WorkerID, Path, Directory, xPaths);
		}
	}
}
