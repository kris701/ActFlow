using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using DatabaseSharp;
using DatabaseSharp.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ActFlow.Integrations.DatabaseSharp.Workers
{
	public class InsertWorkflowFromDatabaseWorker : BaseWorker<InsertWorkflowFromDatabaseActivity>
	{
		[Required]
		public string ConnectionString { get; set; }
		
		private readonly IDBClient _dBClient;

		public InsertWorkflowFromDatabaseWorker(string id, string connectionString) : base(id)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(connectionString);
		}

		public InsertWorkflowFromDatabaseWorker(string connectionString)
		{
			ConnectionString = connectionString;
			_dBClient = new DBClient(ConnectionString);
		}

		public override async Task<WorkerResult> Execute(InsertWorkflowFromDatabaseActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act.HasInserted)
				return new WorkerResult();

			var args = new List<ISQLParameter>();
			foreach (var key in act.Arguments.Keys)
				args.Add(new SQLParam(key, act.Arguments[key]));

			var result = await _dBClient.ExecuteAsync(
				act.TargetSTP,
				args);
			var chainStr = result[0][0].GetValue<string>("Script");
			var chain = JsonSerializer.Deserialize<Workflow>(chainStr);
			if (chain is Workflow script)
				state.Workflow.Activities.InsertRange(state.ActivityIndex + 1, chain.Activities);
			else
				throw new Exception("Could not deserialize database result to ActScript!");

			act.HasInserted = true;

			return new WorkerResult();
		}
	}
}
