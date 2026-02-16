using ActChain.Actions.ML.NET.Actions;
using ActChain.Actions.ML.NET.Classifiers.ML.NET;
using ActChain.Actions.ML.NET.Classifiers.ML.NET.Models;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Executors
{
	[JsonDerivedType(typeof(TrainTextClassifierExecutor), typeDiscriminator: nameof(TrainTextClassifierExecutor))]
	public class TrainTextClassifierExecutor : BaseActionExecutor<TrainTextClassifierAction>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public TrainTextClassifierExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(TrainTextClassifierAction act, ActScriptState state)
		{
			var data = JsonSerializer.Deserialize<List<ModelInput>>(act.Data);
			if (data != null)
			{
				_classifier.ClassifierLog = "";
				await _classifier.Train(data, act.ModelName);
				state.AppendToLog(_classifier.ClassifierLog);
			}

			return new ExecutorResult(new EmptyContext());
		}
	}
}
