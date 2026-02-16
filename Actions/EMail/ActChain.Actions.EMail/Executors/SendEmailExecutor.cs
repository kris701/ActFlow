using ActChain.Actions.EMail.Actions;
using ActChain.Actions.EMail.Contexts;
using ActChain.Actions.EMail.EMail;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;

namespace ActChain.Actions.EMail.Executors
{
	public class SendEmailExecutor : BaseWorker<SendEmailAction>
	{
		public OutlookMailService MailService { get; set; }

		public SendEmailExecutor(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(SendEmailAction act, ActScriptState state, CancellationToken token)
		{
			if (act.Answer is MailContext answer)
				await MailService.SendAsync(answer);
			else
				throw new Exception("Mail context must be a MailModel one!");

			return new WorkerResult(act.Answer);
		}
	}
}
