using ActFlow.Integrations.XML.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ActFlow.Integrations.XML.Workers
{
	public class ExtractValuesFromXMLFileWorker : BaseWorker<ExtractValuesFromXMLFileActivity>
	{
		public ExtractValuesFromXMLFileWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ExtractValuesFromXMLFileActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var txt = await LoadFile(act.Path, act.Directory, tmpDirectory, token);
			var xmlText = XElement.Parse(txt);
			var results = new Dictionary<string, string>();
			foreach (var key in act.XPaths.Keys)
			{
				var value = xmlText.XPathSelectElement(act.XPaths[key]);
				var valueStr = value?.Value;
				if (valueStr == null)
					throw new Exception($"Could not find path '{act.XPaths[key]}' in the XML data!");
				results.Add(key, valueStr);
			}

			return new WorkerResult(new DictionaryContext() { Values = results });
		}
	}
}
