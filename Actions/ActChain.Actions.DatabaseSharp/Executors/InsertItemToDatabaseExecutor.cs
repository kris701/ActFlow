using ActChain.Actions.DatabaseSharp.Actions;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using DatabaseSharp;
using DatabaseSharp.Models;
using System.Text.Json.Serialization;

namespace ActChain.Actions.DatabaseSharp.Executors
{
	[JsonDerivedType(typeof(InsertItemToDatabaseExecutor), typeDiscriminator: nameof(InsertItemToDatabaseExecutor))]
	public class InsertItemToDatabaseExecutor : BaseActionExecutor<InsertItemToDatabaseAction>
	{
		public string ConnectionString { get; set; }
		private readonly IDBClient _dBClient;

		public InsertItemToDatabaseExecutor(string iD, string connectionString) : base(iD)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(InsertItemToDatabaseAction act, ActScriptState state)
		{
			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);

			return new ExecutorResult();
		}
	}
}
