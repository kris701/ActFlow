using ActFlow.Integrations.EMail.Activities;
using ActFlow.Integrations.EMail.Contexts;
using ActFlow.Integrations.EMail.EMail;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.EMail.Workers
{
	public class SendEmailWorker : BaseWorker<SendEmailActivity>
	{
		public OutlookMailService MailService { get; set; }

		public SendEmailWorker(string iD, OutlookMailService mailService) : base(iD)
		{
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(SendEmailActivity act, WorkflowState state, CancellationToken token)
		{
			if (act.Answer is MailContext answer)
				await MailService.SendAsync(answer);
			else
				throw new Exception("Mail context must be a MailModel one!");

			return new WorkerResult(act.Answer);
		}
	}
}
