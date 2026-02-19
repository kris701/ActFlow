using NJsonSchema;
using OpenWebUISharp;
using OpenWebUISharp.Models.Query;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.OpenWebUISharp.OpenWebUI
{
	public class OpenWebUIService : IOpenWebUIService
	{
		private readonly IOpenWebUIWrapper _wrapper;

		[JsonPropertyName("url")]
		public string URL { get; set; }
		[JsonPropertyName("apiKey")]
		public string APIKey { get; set; }

		public OpenWebUIService(string uRL, string aPIKey)
		{
			URL = uRL;
			APIKey = aPIKey;
			_wrapper = new OpenWebUIWrapper(aPIKey, uRL);
		}

		public async Task<string> Query(string query, string model) => await Query(query, model, new List<Guid>());
		public async Task<string> Query(string query, string model, List<Guid> collectionsNames)
		{
			var collections = new List<Guid>();
			if (collectionsNames.Count > 0)
			{
				var all = await _wrapper.Knowledgebase.GetAll();
				foreach (var name in collectionsNames)
					collections.Add(all.First(x => x.Name == name.ToString()).ID);
			}

			var result = await _wrapper.Query.Query(
				new Conversation(
					new List<ConversationMessage>()
					{
						new ConversationMessage("user", query)
					}),
				model,
				new ConversationOptions()
				{
					KnowledgebaseIDs = collections
				});
			return result.Message;
		}

		public async Task<T> QueryToJsonObject<T>(string query, string model) where T : notnull => await QueryToJsonObject<T>(query, model, new List<Guid>());
		public async Task<T> QueryToJsonObject<T>(string query, string model, List<Guid> collectionNames) where T : notnull
		{
			var collections = new List<Guid>();
			if (collectionNames.Count > 0)
			{
				var all = await _wrapper.Knowledgebase.GetAll();
				foreach (var name in collectionNames)
					collections.Add(all.First(x => x.Name == name.ToString()).ID);
			}

			var result = await _wrapper.Query.QueryToObject<T>(
				new Conversation(
					new List<ConversationMessage>()
					{
						new ConversationMessage("user", query)
					}),
				model,
				new ConversationOptions()
				{
					KnowledgebaseIDs = collections
				});
			return result;
		}

		public async Task UpsertKnowledgebase(Guid name, string description)
		{
			var knowledgebases = await _wrapper.Knowledgebase.GetAll();
			if (!knowledgebases.Any(x => x.Name == name.ToString()))
				await _wrapper.Knowledgebase.Add(name.ToString(), "desc");
		}

		public async Task DeleteKnowledgebase(Guid name)
		{
			var knowledgebase = await _wrapper.Knowledgebase.GetByName(name.ToString());
			await _wrapper.Knowledgebase.Delete(knowledgebase.ID);
		}

		public async Task UpsertFileInCollection(Guid collectionName, Guid name, string text)
		{
			var knowledgebase = await _wrapper.Knowledgebase.GetByName(collectionName.ToString());
			var target = knowledgebase.Files.FirstOrDefault(x => x.Name == name.ToString());
			if (target != null)
			{
				await _wrapper.Knowledgebase.DeleteFile(target.ID, knowledgebase.ID);
				await _wrapper.Knowledgebase.AddFile(
					text,
					knowledgebase.ID,
					name.ToString());
			}
			else
			{
				await _wrapper.Knowledgebase.AddFile(
					text,
					knowledgebase.ID,
					name.ToString());
			}
		}

		public async Task DeleteFileFromCollection(Guid collectionName, Guid name)
		{
			var knowledgebase = await _wrapper.Knowledgebase.GetByName(collectionName.ToString());
			var target = knowledgebase.Files.FirstOrDefault(x => x.Name == name.ToString());
			if (target != null)
				await _wrapper.Knowledgebase.DeleteFile(target.ID, knowledgebase.ID);
		}
	}
}
