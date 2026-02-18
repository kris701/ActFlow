using ActFlow.Integrations.EMail.Activities;
using ActFlow.Integrations.EMail.Contexts;
using ActFlow.Integrations.EMail.EMail;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.EMail.Workers
{
	public class ReplyToEmailWorker : BaseWorker<ReplyToEmailActivity>
	{
		public OutlookMailService MailService { get; set; }

		public ReplyToEmailWorker(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(ReplyToEmailActivity act, WorkflowState state, CancellationToken token)
		{
			if (act.Answer is MailContext answer)
				await MailService.ReplyAsync(answer, act.ToMessageID);
			else
				throw new Exception("Mail context must be a MailModel one!");
			return new WorkerResult(act.Answer);
		}
	}
}
