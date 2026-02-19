using System.Text.Json.Serialization;

namespace ActFlow.Integrations.OpenWebUISharp.OpenWebUI
{
	[JsonDerivedType(typeof(OpenWebUIService), typeDiscriminator: "OpenWebUIService")]
	public interface IOpenWebUIService
	{
		[JsonPropertyName("url")]
		public string URL { get; set; }
		[JsonPropertyName("apiKey")]
		public string APIKey { get; set; }

		public Task<string> Query(string query, string model);
		public Task<string> Query(string query, string model, List<Guid> collections);
		public Task<T> QueryToJsonObject<T>(string query, string model) where T : notnull;
		public Task<T> QueryToJsonObject<T>(string query, string model, List<Guid> collections) where T : notnull;

		public Task UpsertKnowledgebase(Guid name, string description);
		public Task DeleteKnowledgebase(Guid name);

		public Task UpsertFileInCollection(Guid collectionName, Guid name, string text);
		public Task DeleteFileFromCollection(Guid collectionName, Guid name);
	}
}
