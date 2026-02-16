using ActChain.Actions.DatabaseSharp.Actions;
using ActChain.Actions.DatabaseSharp.Helpers;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using DatabaseSharp;
using DatabaseSharp.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Executors
{
	public class FetchItemsFromDatabaseExecutor : BaseActionExecutor<FetchItemsFromDatabaseAction>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		public FetchItemsFromDatabaseExecutor(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(FetchItemsFromDatabaseAction act, ActScriptState state, CancellationToken token)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);

			var jsonResult = JsonSerializer.Serialize(result[0].FillAll(TypeHelpers.ByName(act.TargetDeserializationType)));

			return new ExecutorResult(new StringContext() { Text = jsonResult.Replace("\"", "\\\"") });
		}
	}
}
