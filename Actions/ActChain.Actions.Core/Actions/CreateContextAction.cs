using ActChain.Models.Actions;
using ActChain.Models.Contexts;
using System.Text.Json.Serialization;

namespace ActChain.Actions.Core.Actions
{
	[JsonDerivedType(typeof(CreateContextAction), typeDiscriminator: nameof(CreateContextAction))]
	public class CreateContextAction : IAIAction
	{
		public string Name { get; set; } = "createcontext";
		public string ExecutorID { get; set; } = "default";
		public IActionContext Context { get; set; }
		[JsonIgnore]
		public CancellationToken? Token { get; set; }

		public CreateContextAction(string name, string executorId, IActionContext context)
		{
			Name = name;
			ExecutorID = executorId;
			Context = context;
		}

		public IAIAction Clone() => new CreateContextAction(Name, ExecutorID, Context);
	}
}
