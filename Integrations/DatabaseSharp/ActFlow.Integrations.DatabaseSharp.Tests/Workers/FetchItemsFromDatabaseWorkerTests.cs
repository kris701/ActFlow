using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Integrations.DatabaseSharp.Tests.Mocks;
using ActFlow.Integrations.DatabaseSharp.Workers;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;
using DatabaseSharp.Models;

namespace ActFlow.Integrations.DatabaseSharp.Tests.Workers
{
	[TestClass]
	public class FetchItemsFromDatabaseWorkerTests
	{
		[TestMethod]
		public async Task Can_FetchData()
		{
			// ARRANGE
			var resultToGiveDataset = new System.Data.DataSet();
			resultToGiveDataset.ReadXml("TestFiles/books.xml");
			var resultToGive = new DatabaseResult(resultToGiveDataset);
			var dbClient = new MockDBClient();
			dbClient.ResultToGive = resultToGive;
			var worker = new ExecuteSTPWorker("", dbClient);
			var activity = new ExecuteSTPActivity("", "", "", new Dictionary<string, string>(), 0,
				new Dictionary<string, string>() {
					{ "author", "authorkey" },
					{ "price", "pricekey" },
					{ "genre", "genrekey" }
				});

			// ACT
			var result = await worker.Execute(activity, new WorkflowState(), new CancellationToken(), "");

			// ASSERT
			Assert.IsExactInstanceOfType<ListDictionaryContext>(result.Context);
			Assert.HasCount(12, ((ListDictionaryContext)result.Context).Values);
		}
	}
}
