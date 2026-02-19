using ActFlow.Integrations.EMail.Activities;
using ActFlow.Integrations.EMail.EMail;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;

namespace ActFlow.Integrations.EMail.Workers
{
	public class WaitForEmailWorker : BaseWorker<WaitForEmailActivity>
	{
		public int WaitDelayMs { get; set; }
		public OutlookMailService MailService { get; set; }

		public WaitForEmailWorker(string iD, int waitDelayMs, OutlookMailService mailService) : base(iD)
		{
			WaitDelayMs = waitDelayMs;
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(WaitForEmailActivity act, WorkflowState state, CancellationToken token)
		{
			var startTime = DateTime.UtcNow;
			while (true)
			{
				var mails = await MailService.RecentMailsAsync(startTime);
				mails.RemoveAll(x => x.RecievedAt <= startTime);
				mails.RemoveAll(x => x.Sender != act.SenderEmail);
				foreach (var mail in mails)
					if (mail.ConversationID == act.ConversationID)
						return new WorkerResult(mail);

				if (state.Status != WorkflowStatuses.AwaitingUpdate)
				{
					state.Status = WorkflowStatuses.AwaitingUpdate;
					await state.Update();
				}

				await Task.Delay(WaitDelayMs, token);
				if (token.IsCancellationRequested)
					return new WorkerResult();
			}
		}
	}
}
