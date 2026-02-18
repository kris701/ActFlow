using ActFlow.Integrations.ML.NET.Activity;
using ActFlow.Integrations.ML.NET.Classifiers.ML.NET;
using ActFlow.Integrations.ML.NET.Classifiers.ML.NET.Models;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Text.Json;

namespace ActFlow.Integrations.ML.NET.Worker
{
	public class TrainTextClassifierWorker : BaseWorker<TrainTextClassifierActivity>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public TrainTextClassifierWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(TrainTextClassifierActivity act, WorkflowState state, CancellationToken token)
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
