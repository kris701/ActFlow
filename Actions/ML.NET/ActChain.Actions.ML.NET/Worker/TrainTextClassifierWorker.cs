using ActChain.Actions.ML.NET.Actions;
using ActChain.Actions.ML.NET.Classifiers.ML.NET;
using ActChain.Actions.ML.NET.Classifiers.ML.NET.Models;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Executors
{
	public class TrainTextClassifierWorker : BaseWorker<TrainTextClassifierActivity>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public TrainTextClassifierWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(TrainTextClassifierActivity act, ActScriptState state, CancellationToken token)
		{
			var data = JsonSerializer.Deserialize<List<ModelInput>>(act.Data);
			if (data != null)
			{
				_classifier.ClassifierLog = "";
				await _classifier.Train(data, act.ModelName);
				state.AppendToLog(_classifier.ClassifierLog);
			}

			return new WorkerResult(new EmptyContext());
		}
	}
}
