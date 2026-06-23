using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.CreateUploadSession;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.Integrations.Sharepoint.Services
{
	public class SharepointService
	{
		[Required]
		public string ClientID { get; }
		[Required]
		public string TenantID { get; }
		[Required]
		public string ClientSecret { get; }

		private readonly GraphServiceClient _client;

		public SharepointService(string clientID, string tenantId, string clientSecret)
		{
			var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider(clientID, clientSecret, tenantId));
			_client = new GraphServiceClient(authenticationProvider);
			ClientID = clientID;
			TenantID = tenantId;
			ClientSecret = clientSecret;
		}

		public async Task UploadFile(Stream fileStream, string driveId, string folderId, string fileName)
		{
			CreateUploadSessionPostRequestBody uploadSessionRequestBody = new CreateUploadSessionPostRequestBody
			{
				Item = new DriveItemUploadableProperties
				{
					AdditionalData = new Dictionary<string, object>
				{
					{ "@microsoft.graph.conflictBehavior", "rename" }, // fail, replace, or rename
                },
				},
			};
			UploadSession uploadSession = await _client.Drives[driveId]
															.Items[folderId]
															.ItemWithPath(fileName)
															.CreateUploadSession
															.PostAsync(uploadSessionRequestBody);
			int maxSliceSize = 320 * 1024;
			LargeFileUploadTask<DriveItem> fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, fileStream, maxSliceSize, _client.RequestAdapter);
			long totalLength = fileStream.Length;
			UploadResult<DriveItem> uploadResult = await fileUploadTask.UploadAsync();
		}
	}
}
