namespace ActChain.Models.Contexts
{
	public interface IContext
	{
		public string GetContent();
		public IContext Clone();
		public Dictionary<string, string> GetContextValues();
	}
}
