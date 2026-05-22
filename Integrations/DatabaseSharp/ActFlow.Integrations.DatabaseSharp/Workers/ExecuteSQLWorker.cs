using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using DatabaseSharp;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.DatabaseSharp.Workers
{
	public class ExecuteSQLWorker : BaseWorker<ExecuteSQLActivity>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		[JsonConstructor]
		public ExecuteSQLWorker(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public ExecuteSQLWorker(string iD, IDBClient dbClient) : base(iD)
		{
			ConnectionString = dbClient.ConnectionString;
			_dBClient = dbClient;
		}

		public override async Task<WorkerResult> Execute(ExecuteSQLActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var result = await _dBClient.ExecuteFreeAsync(
				act.SQL);

			var results = new List<Dictionary<string, string>>();

			if (result.Count >= act.ResultTable)
			{
				var targetTable = result[act.ResultTable];
				foreach (var row in targetTable)
				{
					var newItem = new Dictionary<string, string>();
					foreach (var mapKey in act.ResultMap.Keys)
					{
						var value = row.GetValue<string>(mapKey);
						newItem.Add(act.ResultMap[mapKey], value);
					}
					if (newItem.Count > 0)
						results.Add(newItem);
				}
			}

			return new WorkerResult(new ListDictionaryContext() { Values = results });
		}
	}
}
