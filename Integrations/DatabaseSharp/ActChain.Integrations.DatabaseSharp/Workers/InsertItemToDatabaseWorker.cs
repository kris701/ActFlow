using ActChain.Integrations.DatabaseSharp.Activities;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;
using DatabaseSharp;
using DatabaseSharp.Models;

namespace ActChain.Integrations.DatabaseSharp.Workers
{
	public class InsertItemToDatabaseWorker : BaseWorker<InsertItemToDatabaseActivity>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		public InsertItemToDatabaseWorker(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public override async Task<WorkerResult> Execute(InsertItemToDatabaseActivity act, ActScriptState state, CancellationToken token)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);

			return new WorkerResult();
		}
	}
}
