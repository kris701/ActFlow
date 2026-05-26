using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using DatabaseSharp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.DatabaseSharp.Workers
{
	public class ExecuteSQLToFileWorker : BaseWorker<ExecuteSQLToFileActivity>
	{
		[Required]
		public string ConnectionString { get; set; }
		
		private readonly IDBClient _dBClient;

		[JsonConstructor]
		public ExecuteSQLToFileWorker(string connectionString)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(ConnectionString);
		}

		public ExecuteSQLToFileWorker(IDBClient dbClient)
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
				await SaveFile(act.Path, DataTableToCSV(targetTable.ToDataTable()), act.Directory, tmpDirectory, token);
			}

			return new WorkerResult();
		}

		private string DataTableToCSV(DataTable table)
		{
			StringBuilder sb = new StringBuilder();

			IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
											  Select(column => column.ColumnName);
			sb.AppendLine(string.Join(",", columnNames));

			foreach (DataRow row in table.Rows)
			{
				IEnumerable<string> fields = row.ItemArray.Select(field =>
				  string.Concat("\"", field.ToString().Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t"), "\""));
				sb.AppendLine(string.Join(",", fields));
			}

			return sb.ToString();
		}
	}
}
