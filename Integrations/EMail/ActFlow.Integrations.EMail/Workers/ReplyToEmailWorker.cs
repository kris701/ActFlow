using ActFlow.Integrations.EMail.Activities;
using ActFlow.Integrations.EMail.Contexts;
using ActFlow.Integrations.EMail.EMail;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Integrations.EMail.Workers
{
	public class ReplyToEmailWorker : BaseWorker<ReplyToEmailActivity>
	{
		[Required]
		public OutlookMailService MailService { get; set; }

		[JsonConstructor]
		public ReplyToEmailWorker(OutlookMailService mailService)
		{
			MailService = mailService;
		}

		public override async Task<WorkerResult> Execute(ReplyToEmailActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act.Answer is MailContext answer)
				await MailService.ReplyAsync(answer, act.ToMessageID);
			else
				throw new Exception("Mail context must be a MailModel one!");
			return new WorkerResult(act.Answer);
		}
	}
}
