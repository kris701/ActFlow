using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using DatabaseSharp;
using DatabaseSharp.Models;

namespace ActFlow.Integrations.DatabaseSharp.Workers
{
	public class FetchItemsFromDatabaseWorker : BaseWorker<FetchItemsFromDatabaseActivity>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		public FetchItemsFromDatabaseWorker(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public override async Task<WorkerResult> Execute(FetchItemsFromDatabaseActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);

			var results = new List<Dictionary<string, string>>();

			if (result.Count >= act.ResultTable)
			{
				var targetTable = result[act.ResultTable];
				var newItem = new Dictionary<string, string>();
				foreach (var row in targetTable)
				{
					foreach (var mapKey in act.ResultMap.Keys)
					{
						var value = row.GetValue<string>(mapKey);
						newItem.Add(act.ResultMap[mapKey], value);
					}
				}
				results.Add(newItem);
			}

			return new WorkerResult(new ListDictionaryContext() { Values = results });
		}
	}
}
