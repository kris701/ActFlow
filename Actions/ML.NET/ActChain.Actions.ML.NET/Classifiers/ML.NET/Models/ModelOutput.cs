using Microsoft.ML.Data;

namespace ActChain.Actions.ML.NET.Classifiers.ML.NET.Models
{
	public class ModelOutput
	{
		[ColumnName(@"PredictedLabel")]
		public string PredictedLabel { get; set; }

		[ColumnName(@"Score")]
		public float[] Score { get; set; }
	}
}
