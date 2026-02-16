using ActChain.Integrations.ML.NET.Classifiers.ML.NET.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;

namespace ActChain.Integrations.ML.NET.Classifiers.ML.NET
{
	public class TextClassifier : IClassifier<ModelInput, ClassificationResult>
	{
		public string ClassifierLog { get; internal set; } = "";

		public async Task Train(List<ModelInput> items, string modelName)
		{
			if (!Directory.Exists("trainingmodels"))
				Directory.CreateDirectory("trainingmodels");

			AppendToLog("Training info:");
			AppendToLog($"\tTotal items to train on: {items.Count}");
			AppendToLog($"\tTotal classes:           {items.DistinctBy(x => x.Label).Count()}");
			AppendToLog();

			// Initialize MLContext
			MLContext mlContext = new(seed: 0)
			{
				GpuDeviceId = 0,
				FallbackToCpu = true
			};

			AppendToLog("Loading data...");
			IDataView dataView = mlContext.Data.LoadFromEnumerable(items);

			AppendToLog("Splitting dataset...");
			DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.5);
			IDataView trainData = dataSplit.TrainSet;
			IDataView testData = dataSplit.TestSet;

			AppendToLog($"Creating training pipeline...");
			var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
										outputColumnName: "Label",
										inputColumnName: "Label")
									.Append(mlContext.MulticlassClassification.Trainers.TextClassification(
										sentence1ColumnName: "Value"))
									.Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

			AppendToLog($"Training model...");
			ITransformer model = pipeline.Fit(trainData);

			AppendToLog($"Evaluating model performance...");
			IDataView transformedTest = model.Transform(testData);
			MulticlassClassificationMetrics metrics = mlContext.MulticlassClassification.Evaluate(transformedTest);

			AppendToLog($"\tMacro Accuracy: {metrics.MacroAccuracy}");
			AppendToLog($"\tMicro Accuracy: {metrics.MicroAccuracy}");
			AppendToLog($"\tLog Loss:       {metrics.LogLoss}");
			AppendToLog();

			AppendToLog($"Saving trained model...");
			var targetFile = $"trainingmodels/{modelName}.zip";
			var targetFileInfo = new FileInfo(targetFile);
			if (!Directory.Exists(targetFileInfo.Directory!.FullName))
				Directory.CreateDirectory(targetFileInfo.Directory!.FullName);
			mlContext.Model.Save(model, dataView.Schema, targetFile);

			AppendToLog($"Training sequence complete!");
		}

		public ClassificationResult Predict(ModelInput item, string modelName)
		{
			if (!Directory.Exists("trainingmodels"))
				Directory.CreateDirectory("trainingmodels");
			if (!File.Exists($"trainingmodels/{modelName}.zip"))
				throw new FileNotFoundException("Model not found! You must train a model before you can predict with it.");

			// Initialize MLContext
			MLContext mlContext = new()
			{
				GpuDeviceId = 0,
				FallbackToCpu = true
			};

			DataViewSchema modelSchema;
			ITransformer trainedModel = mlContext.Model.Load($"trainingmodels/{modelName}.zip", out modelSchema);
			PredictionEngine<ModelInput, ModelOutput> engine =
				mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

			var result = engine.Predict(item);

			return new ClassificationResult() { Label = result.PredictedLabel, Score = result.Score[0] };
		}

		private void AppendToLog() => AppendToLog("");
		private void AppendToLog(string text)
		{
			ClassifierLog += $"{text}{Environment.NewLine}";
		}
	}
}
