using ActFlow.Integrations.EMail.Activities;
using ActFlow.Integrations.EMail.EMail;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.EMail.Workers
{
	public class WaitForEmailWorker : BaseWorker<WaitForEmailActivity>
	{
		[Required]
		public int WaitDelayMs { get; set; }
		[Required]
		public OutlookMailService MailService { get; set; }

		[JsonConstructor]
		public WaitForEmailWorker(int waitDelayMs, OutlookMailService mailService)
		{
			WaitDelayMs = waitDelayMs;
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(WaitForEmailActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
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
