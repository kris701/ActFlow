using DatabaseSharp;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;

namespace ActFlow.Integrations.DatabaseSharp.Tests.Mocks
{
	public class MockDBClient : IDBClient
	{
		public string ConnectionString { get; set; }
		public DatabaseResult ResultToGive { get; set; }
		public Dictionary<string, IDatabaseSerializer> Serializers => new Dictionary<string, IDatabaseSerializer>();

		public async Task<DatabaseResult> ExecuteAsync(string procedureName, List<ISQLParameter>? parameters = null) => ResultToGive;
		public async Task<DatabaseResult> ExecuteAsync(string procedureName, object item) => ResultToGive;
	}
}
