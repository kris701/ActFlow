using Microsoft.ML.Data;

namespace ActChain.Integrations.ML.NET.Classifiers.ML.NET.Models
{
	public class ModelInput
	{
		[LoadColumn(0)]
		[ColumnName(@"Value")]
		public string Value { get; set; }

		[LoadColumn(1)]
		[ColumnName(@"Label")]
		public string Label { get; set; }
	}
}
