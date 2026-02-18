using ActFlow.Integrations.ML.NET.Activity;
using ActFlow.Integrations.ML.NET.Classifiers.ML.NET;
using ActFlow.Integrations.ML.NET.Classifiers.ML.NET.Models;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.ML.NET.Worker
{
	public class ClassifyTextWorker : BaseWorker<ClassifyTextActivity>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public ClassifyTextWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ClassifyTextActivity act, WorkflowState state, CancellationToken token)
		{
			var predict = _classifier.Predict(new ModelInput() { Value = act.Text }, act.Model);
			return new WorkerResult(
				new StringContext() { Text = predict.Label });
		}
	}
}
