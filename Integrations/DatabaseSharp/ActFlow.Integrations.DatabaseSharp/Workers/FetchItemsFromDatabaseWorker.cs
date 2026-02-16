using ActChain.Integrations.DatabaseSharp.Activities;
using ActChain.Integrations.DatabaseSharp.Helpers;
using ActChain.Models.Contexts;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using DatabaseSharp;
using DatabaseSharp.Models;
using System.Text.Json;

namespace ActChain.Integrations.DatabaseSharp.Workers
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

		public override async Task<WorkerResult> Execute(FetchItemsFromDatabaseActivity act, WorkflowState state, CancellationToken token)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);

			var jsonResult = JsonSerializer.Serialize(result[0].FillAll(TypeHelpers.ByName(act.TargetDeserializationType)));

			return new WorkerResult(new StringContext() { Text = jsonResult.Replace("\"", "\\\"") });
		}
	}
}
