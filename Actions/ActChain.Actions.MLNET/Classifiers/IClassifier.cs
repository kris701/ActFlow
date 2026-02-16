namespace ActChain.Actions.MLNET.Classifiers
{
	public interface IClassifier<Tin, TOut>
	{
		public string ClassifierLog { get; }

		public Task Train(List<Tin> items, string modelName);
		public TOut Predict(Tin item, string modelName);
	}
}
