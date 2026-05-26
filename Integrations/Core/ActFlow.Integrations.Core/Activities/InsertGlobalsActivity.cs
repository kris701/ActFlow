using ActFlow.Models.Activities;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Core.Activities
{
	public class InsertGlobalsActivity : IActivity
	{
		public string Name { get; set; } = "insertglobal";
		public string WorkerID { get; set; } = "default";
		[Required]
		public Dictionary<string, string> Arguments { get; set; }

		public IActivity Clone()
		{
			var arguments = new Dictionary<string, string>();
			foreach (var key in Arguments.Keys)
				arguments.Add(key, Arguments[key]);
			return new InsertGlobalsActivity()
			{
				Name = Name,
				WorkerID = WorkerID,
				Arguments = arguments
			};
		}
	}
}
