using ActFlow.Models.Contexts;

namespace ActFlow.Models.Activities
{
	public interface IHumanInput
	{
		public void Apply(IContext context);
	}
}
