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

		public IActivity Clone()
		{
			var xPaths = new Dictionary<string, string>();
			foreach (var key in XPaths.Keys)
				xPaths.Add(key, XPaths[key]);
			return new ExtractValuesFromXMLActivity()
			{
				Name = Name,
				WorkerID = WorkerID,
				XML = XML,
				XPaths = xPaths
			};
		}
	}
}
