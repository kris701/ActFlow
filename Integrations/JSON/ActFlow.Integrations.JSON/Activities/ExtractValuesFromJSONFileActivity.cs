using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.JSON.Activities
{
	public class ExtractValuesFromJSONFileActivity : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsonpathtext";
		public string WorkerID { get; set; } = "default";

		public string Path { get; set; }
		public FileDirectories Directory { get; set; } = FileDirectories.Temporary;
		[JsonPropertyName("jsonPaths")]
		public Dictionary<string, string> JSONPaths { get; set; }

		public ExtractValuesFromJSONFileActivity(string name, string workerId, string path, FileDirectories directory, Dictionary<string, string> jsonPaths)
		{
			Name = name;
			WorkerID = workerId;
			Path = path;
			Directory = directory;
			JSONPaths = jsonPaths;
		}

		public IActivity Clone()
		{
			var jsonPaths = new Dictionary<string, string>();
			foreach (var key in JSONPaths.Keys)
				jsonPaths.Add(key, JSONPaths[key]);
			return new ExtractValuesFromJSONFileActivity(Name, WorkerID, Path, Directory, jsonPaths);
		}
	}
}
