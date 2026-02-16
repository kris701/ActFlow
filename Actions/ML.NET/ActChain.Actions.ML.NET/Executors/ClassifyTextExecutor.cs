using ActChain.Actions.ML.NET.Actions;
using ActChain.Actions.ML.NET.Classifiers.ML.NET;
using ActChain.Actions.ML.NET.Classifiers.ML.NET.Models;
using ActChain.Models.Contexts;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Executors
{
	public class ClassifyTextExecutor : BaseWorker<ClassifyTextAction>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public ClassifyTextExecutor(string iD) : base(iD)
		{
		}

		public override async Task<WorkerResult> Execute(ClassifyTextAction act, ActScriptState state, CancellationToken token)
		{
			var predict = _classifier.Predict(new ModelInput() { Value = act.Text }, act.Model);
			return new WorkerResult(
				new StringContext() { Text = predict.Label });
		}
	}
}
