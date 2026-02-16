using ActChain.Actions.Outlook.Actions;
using ActChain.Actions.Outlook.EMail;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;

namespace ActChain.Actions.Outlook.Executors
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
