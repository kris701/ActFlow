using ActChain.Actions.EMail.Actions;
using ActChain.Actions.EMail.Contexts;
using ActChain.Actions.EMail.EMail;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;

namespace ActChain.Actions.EMail.Executors
{
	public class ReplyToEmailExecutor : BaseActionExecutor<ReplyToEmailAction>
	{
		public OutlookMailService MailService { get; set; }

		public ReplyToEmailExecutor(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(ReplyToEmailAction act, ActScriptState state, CancellationToken token)
		{
			if (act.Answer is MailContext answer)
				await MailService.ReplyAsync(answer, act.ToMessageID);
			else
				throw new Exception("Mail context must be a MailModel one!");
			return new ExecutorResult(act.Answer);
		}
	}
}
