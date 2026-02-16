using ActChain.Integrations.DatabaseSharp.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using DatabaseSharp;
using DatabaseSharp.Models;
using System.Text.Json;

namespace ActChain.Integrations.DatabaseSharp.Workers
{
	public class InsertChainFromDatabaseWorker : BaseWorker<InsertChainFromDatabaseActivity>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		public InsertChainFromDatabaseWorker(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public override async Task<WorkerResult> Execute(InsertChainFromDatabaseActivity act, ActScriptState state, CancellationToken token)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);
			var chainStr = result[0][0].GetValue<string>("Script");
			var chain = JsonSerializer.Deserialize<ActScript>(chainStr);
			if (chain is ActScript script)
				state.Script.Stages.InsertRange(state.Stage + 1, chain.Stages);
			else
				throw new Exception("Could not deserialize database result to ActScript!");

			return new WorkerResult();
		}
	}
}
