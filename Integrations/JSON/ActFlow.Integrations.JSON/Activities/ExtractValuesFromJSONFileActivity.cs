using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

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
	}
}
