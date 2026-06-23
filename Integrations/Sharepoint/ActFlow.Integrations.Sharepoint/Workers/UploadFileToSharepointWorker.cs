using ActFlow.Integrations.Sharepoint.Activities;
using ActFlow.Integrations.Sharepoint.Services;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Sharepoint.Workers
{
	public class UploadFileToSharepointWorker : BaseWorker<UploadFileToSharepointActivity>
	{
		[Required]
		public SharepointService Service { get; set; }

		public UploadFileToSharepointWorker(SharepointService service)
		{
			Service = service;
		}

		public override async Task<WorkerResult> Execute(UploadFileToSharepointActivity act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			using (Stream fileStream = LoadFileStream(act.Path, act.Directory, tmpDirectory, state, token))
			{
				await Service.UploadFile(fileStream, act.DriveID, act.FolderID, GetFileName(act.Path, act.Directory, tmpDirectory));
			}

			return new WorkerResult();
		}
	}
}
