using ActChain.Actions.EMail.Actions;
using ActChain.Actions.EMail.Contexts;
using ActChain.Actions.EMail.EMail;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;

namespace ActChain.Actions.EMail.Executors
{
	public class SendEmailExecutor : BaseActionExecutor<SendEmailAction>
	{
		public OutlookMailService MailService { get; set; }

		public SendEmailExecutor(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(SendEmailAction act, ActScriptState state)
		{
			if (act.Answer is MailContext answer)
				await MailService.SendAsync(answer);
			else
				throw new Exception("Mail context must be a MailModel one!");

			return new ExecutorResult(act.Answer);
		}
	}
}
