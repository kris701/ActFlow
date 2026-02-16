using ActChain.Actions.ML.NET.Actions;
using ActChain.Actions.ML.NET.Classifiers.ML.NET;
using ActChain.Actions.ML.NET.Classifiers.ML.NET.Models;
using ActChain.Models.Contexts;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.ML.NET.Executors
{
	[JsonDerivedType(typeof(ClassifyTextExecutor), typeDiscriminator: nameof(ClassifyTextExecutor))]
	public class ClassifyTextExecutor : BaseActionExecutor<ClassifyTextAction>
	{
		private readonly TextClassifier _classifier = new TextClassifier();

		public ClassifyTextExecutor(string iD) : base(iD)
		{
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ClassifyTextAction act, ActScriptState state)
		{
			var predict = _classifier.Predict(new ModelInput() { Value = act.Text }, act.Model);
			return new ExecutorResult(
				new StringContext() { Text = predict.Label });
		}
	}
}
