using ActFlow.Models.Activities;

namespace ActFlow.Integrations.XML.Activities
{
	public class ExtractValuesFromXMLActivity : IActivity
	{
		public string Name { get; set; } = "extractvaluesfromxml";
		public string WorkerID { get; set; } = "default";
		public string XML { get; set; } = "";
		public Dictionary<string, string> XPaths { get; set; } = new Dictionary<string, string>();

		public ExtractValuesFromXMLActivity(string name, string workerID, string xML, Dictionary<string, string> xPaths)
		{
			Name = name;
			WorkerID = workerID;
			XML = xML;
			XPaths = xPaths;
		}

		public IActivity Clone()
		{
			var xPaths = new Dictionary<string, string>();
			foreach (var key in XPaths.Keys)
				xPaths.Add(key, XPaths[key]);
			return new ExtractValuesFromXMLActivity(Name, WorkerID, XML, xPaths);
		}
	}
}
