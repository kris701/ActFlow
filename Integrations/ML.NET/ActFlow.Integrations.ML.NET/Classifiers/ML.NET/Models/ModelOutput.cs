using Microsoft.ML.Data;

namespace ActFlow.Integrations.ML.NET.Classifiers.ML.NET.Models
{
	public class ModelOutput
	{
		[ColumnName(@"PredictedLabel")]
		public string PredictedLabel { get; set; }

		[ColumnName(@"Score")]
		public float[] Score { get; set; }
	}
}
