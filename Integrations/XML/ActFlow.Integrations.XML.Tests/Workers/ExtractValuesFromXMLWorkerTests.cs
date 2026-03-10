using ActFlow.Integrations.XML.Activities;
using ActFlow.Integrations.XML.Workers;
using ActFlow.Models.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.Integrations.XML.Tests.Workers
{
	[TestClass]
	public class ExtractValuesFromXMLWorkerTests
	{
		[TestMethod]
		public async Task Can_ExtractFromXML()
		{
			// ARRANGE
			var xmlText = File.ReadAllText("TestFiles/books.xml");
			var worker = new ExtractValuesFromXMLWorker("");
			var activity = new ExtractValuesFromXMLActivity("","", xmlText, new Dictionary<string, string>() {
				{ "v1", "//book[last()]/author" },
				{ "v2", "//book[@id='bk102'][1]/author" }
			});

			// ACT
			var result = await worker.Execute(activity, new Models.Workflows.WorkflowState(), new CancellationToken(), "");

			// ASSERT
			Assert.IsExactInstanceOfType<DictionaryContext>(result.Context);
			Assert.AreEqual("Galos, Mike", ((DictionaryContext)result.Context).Values["v1"]);
			Assert.AreEqual("Ralls, Kim", ((DictionaryContext)result.Context).Values["v2"]);
		}
	}
}
