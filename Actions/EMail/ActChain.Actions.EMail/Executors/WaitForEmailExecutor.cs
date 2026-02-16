using ActChain.Actions.EMail.Actions;
using ActChain.Actions.EMail.EMail;
using ActChain.Models.Executors;
using ActChain.Models.Scripts;

namespace ActChain.Actions.EMail.Executors
{
	public class WaitForEmailExecutor : BaseActionExecutor<WaitForEmailAction>
	{
		public int WaitDelayMs { get; set; }
		public OutlookMailService MailService { get; set; }

		public WaitForEmailExecutor(string iD, int waitDelayMs, OutlookMailService mailService) : base(iD)
		{
			WaitDelayMs = waitDelayMs;
			MailService = mailService;
		}

		public override async Task<ExecutorResult> ExecuteActionAsync(WaitForEmailAction act, ActScriptState state)
		{
			var startTime = DateTime.UtcNow;
			while (true)
			{
				var mails = await MailService.RecentMailsAsync(startTime);
				mails.RemoveAll(x => x.RecievedAt <= startTime);
				mails.RemoveAll(x => x.Sender != act.SenderEmail);
				foreach (var mail in mails)
					if (mail.ConversationID == act.ConversationID)
						return new ExecutorResult(mail);

				if (act.Token != null)
				{
					await Task.Delay(WaitDelayMs, (CancellationToken)act.Token);
					if (act.Token.Value.IsCancellationRequested)
						return new ExecutorResult();
				}
				else
					await Task.Delay(WaitDelayMs);
			}
		}
	}
}
