using ActChain.Actions.EMail.Actions;
using ActChain.Actions.EMail.EMail;
using ActChain.Models.Workers;
using ActChain.Models.Scripts;

namespace ActChain.Actions.EMail.Executors
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

		public override async Task<WorkerResult> Execute(WaitForEmailActivity act, ActScriptState state, CancellationToken token)
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

				await Task.Delay(WaitDelayMs, token);
				if (token.IsCancellationRequested)
					return new WorkerResult();
			}
		}
	}
}
