using ActFlow.Models.Activities;

namespace ActFlow.Integrations.SerializableHttps.Activities
{
	public class ExecuteHttpActivity : IActivity
	{
		public string Name { get; set; } = "executehttp";
		public string WorkerID { get; set; } = "default";

		public string Route { get; set; }
		public string Content { get; set; }
		public Dictionary<string, string> Headers { get; set; }
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
