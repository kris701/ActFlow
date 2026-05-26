using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.SerializableHttps.Activities
{
	public class ExecuteHttpActivity : IActivity
	{
		public string Name { get; set; } = "executehttp";
		public string WorkerID { get; set; } = "default";

		[Required] 
		public string Route { get; set; }
		[Required]
		public string Content { get; set; }
		[Required]
		public Dictionary<string, string> Headers { get; set; }
		[Required]
		public HttpTypes Type { get; set; }

		public IActivity Clone()
		{
			var headers = new Dictionary<string, string>();
			foreach (var key in Headers.Keys)
				headers.Add(key, Headers[key]);

			return new ExecuteHttpActivity()
			{
				Name = Name,
				WorkerID = WorkerID,
				Content = Content,
				Route = Route,
				Headers = headers,
				Type = Type
			};
		}
	}
}
