using ActChain.Integrations.ML.NET.Activity;
using ActChain.Integrations.ML.NET.Classifiers.ML.NET;
using ActChain.Integrations.ML.NET.Classifiers.ML.NET.Models;
using ActChain.Models.Contexts;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.ML.NET.Worker
{
	public class ClassifyTextWorker : BaseWorker<ClassifyTextActivity>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public ClassifyTextWorker(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ClassifyTextActivity act, ActScriptState state, CancellationToken token)
		{
			var predict = _classifier.Predict(new ModelInput() { Value = act.Text }, act.Model);
			return new WorkerResult(
				new StringContext() { Text = predict.Label });
		}
	}
}
