using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using DatabaseSharp;
using System.Text;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.DatabaseSharp.Workers
{
	public class ExecuteSQLToFileWorker : BaseWorker<ExecuteSQLToFileActivity>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		[JsonConstructor]
		public ExecuteSQLToFileWorker(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public ExecuteSQLToFileWorker(string iD, IDBClient dbClient) : base(iD)
		{
			ConnectionString = dbClient.ConnectionString;
			_dBClient = dbClient;
		}

		public override async Task<WorkerResult> Execute(ExecuteSQLToFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var result = await _dBClient.ExecuteFreeAsync(
				act.SQL);

			if (result.Count >= act.ResultTable)
			{
				var targetTable = result[act.ResultTable];
				var sb = new StringBuilder();
				var headerCounter = 0;
				foreach (var col in targetTable.Columns)
					sb.Append(col + (headerCounter++ < targetTable.Columns.Count - 1 ? "," : ""));
				sb.AppendLine();

				foreach (var row in targetTable)
				{
					headerCounter = 0;
					foreach (var col in targetTable.Columns)
					{
						var value = row.GetValueOrNull(typeof(string), col);
						if (value is string str)
						{
							str = str.Replace("\r\n", "");
							str = str.Replace("\r", "");
							str = str.Replace("\n", "");
							sb.Append(str + (headerCounter++ < targetTable.Columns.Count - 1 ? "," : ""));
						}
						else
							sb.Append((headerCounter++ < targetTable.Columns.Count - 1 ? "," : ""));
					}
					sb.AppendLine();
				}

				await SaveFile(act.Path, sb.ToString(), act.Directory, tmpDirectory, token);
			}

			return new WorkerResult();
		}
	}
}
