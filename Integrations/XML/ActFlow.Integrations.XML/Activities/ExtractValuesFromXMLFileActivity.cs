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

		public IActivity Clone()
		{
			var xPaths = new Dictionary<string, string>();
			foreach (var key in XPaths.Keys)
				xPaths.Add(key, XPaths[key]);
			return new ExtractValuesFromXMLFileActivity() { 
				Name = Name,
				WorkerID = WorkerID,
				Path = Path,
				Directory = Directory,
				XPaths = xPaths
			};
		}
	}
}
