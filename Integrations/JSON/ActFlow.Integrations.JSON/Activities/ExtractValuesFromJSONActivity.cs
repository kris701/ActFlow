using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.JSON.Activities
{
	public class ExtractValuesFromJSONActivity : IActivity
	{
		public string Name { get; set; } = "extractasinglevaluefromjsontext";
		public string WorkerID { get; set; } = "default";

		[Required]
		public string JSON { get; set; }
		[Required]
		public Dictionary<string, string> JSONPaths { get; set; }

		public IActivity Clone()
		{
			var jsonPaths = new Dictionary<string, string>();
			foreach (var key in JSONPaths.Keys)
				jsonPaths.Add(key, JSONPaths[key]);
			return new ExtractValuesFromJSONActivity()
			{
				Name = Name,
				WorkerID = WorkerID,
				JSON = JSON,
				JSONPaths = jsonPaths
			};
		}
	}
}
