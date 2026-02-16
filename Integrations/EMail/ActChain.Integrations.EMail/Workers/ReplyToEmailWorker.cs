using ActChain.Integrations.EMail.Activities;
using ActChain.Integrations.EMail.Contexts;
using ActChain.Integrations.EMail.EMail;
using ActChain.Models.Scripts;
using ActChain.Models.Workers;

namespace ActChain.Integrations.EMail.Workers
{
	public class ReplyToEmailWorker : BaseWorker<ReplyToEmailActivity>
	{
		public OutlookMailService MailService { get; set; }

		public ReplyToEmailWorker(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(ReplyToEmailActivity act, ActScriptState state, CancellationToken token)
		{
			if (act.Answer is MailContext answer)
				await MailService.ReplyAsync(answer, act.ToMessageID);
			else
				throw new Exception("Mail context must be a MailModel one!");
			return new WorkerResult(act.Answer);
		}
	}
}
