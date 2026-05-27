using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.XML.Activities
{
	public class ExtractValuesFromXMLFileActivity : IActivity
	{
		public string Name { get; set; } = "extractvaluesfromxml";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string Path { get; set; }
		[Required]
		public FileDirectories Directory { get; set; }
		[Required]
		public Dictionary<string, string> XPaths { get; set; }
	}
}
