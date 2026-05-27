using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.XML.Activities
{
	public class ExtractValuesFromXMLActivity : IActivity
	{
		public string Name { get; set; } = "extractvaluesfromxml";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string XML { get; set; }
		[Required]
		public Dictionary<string, string> XPaths { get; set; }
	}
}
