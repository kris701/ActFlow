using ActFlow.Integrations.DatabaseSharp.Activities;
using ActFlow.Integrations.DatabaseSharp.Tests.Mocks;
using ActFlow.Integrations.DatabaseSharp.Workers;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;
using DatabaseSharp.Models;

namespace ActFlow.Integrations.DatabaseSharp.Tests.Workers
{
	[TestClass]
	public class ExecuteSQLToFileWorkerTests
	{
		[TestMethod]
		public async Task Can_Execute()
		{
			// ARRANGE
			var resultToGiveDataset = new System.Data.DataSet();
			resultToGiveDataset.ReadXml("TestFiles/books.xml");
			var resultToGive = new DatabaseResult(resultToGiveDataset);
			var dbClient = new MockDBClient();
			dbClient.ResultToGive = resultToGive;
			var worker = new ExecuteSQLToFileWorker(dbClient);
			var activity = new ExecuteSQLToFileActivity()
			{
				WorkerID = "",
				Name = "",
				SQL = "SELECT ABC",
				Directory = FileDirectories.Temporary,
				Path = "file.csv",
				ResultTable = 0
			};

			// ACT
			var result = await worker.Execute(activity, new WorkflowState(), new CancellationToken(), "");

			// ASSERT
			var csvTxt = File.ReadAllText(activity.Path);
			var expectedTxt = File.ReadAllText("TestFiles/expected.csv");
			Assert.AreEqual(expectedTxt, csvTxt);
		}
	}
}
