using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.JSON.Activities
{
	public class ExtractValuesFromJSONFileActivity : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsonpathtext";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Path { get; set; }
		[Required]
		public FileDirectories Directory { get; set; } = FileDirectories.Temporary;
		[Required]
		public Dictionary<string, string> JSONPaths { get; set; }

		public IActivity Clone()
		{
			var jsonPaths = new Dictionary<string, string>();
			foreach (var key in JSONPaths.Keys)
				jsonPaths.Add(key, JSONPaths[key]);
			return new ExtractValuesFromJSONFileActivity() {
				Name = Name,
				WorkerID = WorkerID,
				Path = Path,
				Directory = Directory,
				JSONPaths = jsonPaths
			};
		}
	}
}
