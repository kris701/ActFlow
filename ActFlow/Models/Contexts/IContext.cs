namespace ActFlow.Models.Contexts
{
	public interface IContext
	{
		public IContext Clone();
		public Dictionary<string, string> GetContextValues();
	}
}
