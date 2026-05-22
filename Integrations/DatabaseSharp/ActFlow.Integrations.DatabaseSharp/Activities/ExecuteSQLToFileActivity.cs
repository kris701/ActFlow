using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;

namespace ActFlow.Integrations.DatabaseSharp.Activities
{
	public class ExecuteSQLToFileActivity : IActivity
	{
		public string Name { get; set; }
		public string WorkerID { get; set; }
		public string SQL { get; set; }
		public int ResultTable { get; set; } = 0;
		public string Path { get; set; }
		public FileDirectories Directory { get; set; }

		public IActivity Clone() => new ExecuteSQLToFileActivity()
		{
			Name = Name,
			WorkerID = WorkerID,
			SQL = SQL,
			ResultTable = ResultTable,
			Path = Path,
			Directory = Directory
		};
	}
}
