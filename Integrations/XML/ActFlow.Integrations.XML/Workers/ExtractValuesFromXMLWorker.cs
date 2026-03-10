using ActFlow.Integrations.XML.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using ToolsSharp;

namespace ActFlow.Integrations.XML.Workers
{
	public class ExtractValuesFromXMLWorker : BaseWorker<ExtractValuesFromXMLActivity>
	{
		public ExtractValuesFromXMLWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ExtractValuesFromXMLActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			var xmlText = XElement.Parse(act.XML);
			var results = new Dictionary<string, string>();
			foreach(var key in act.XPaths.Keys)
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
