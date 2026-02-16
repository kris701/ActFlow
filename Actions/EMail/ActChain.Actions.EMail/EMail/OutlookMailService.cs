using ActChain.Actions.EMail.Contexts;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ActChain.Actions.EMail.EMail
{
	public class OutlookMailService
	{
		public string TargetEmail { get; }
		public int TopN { get; }
		public string ClientID { get; }
		public string TenantID { get; }
		public string ClientSecret { get; }

		private readonly GraphServiceClient _client;

		public OutlookMailService(string targetEmail, int topN, string clientID, string tenantId, string clientSecret)
		{
			var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider(clientID, clientSecret, tenantId));
			_client = new GraphServiceClient(authenticationProvider);
			TargetEmail = targetEmail;
			TopN = topN;
			ClientID = clientID;
			TenantID = tenantId;
			ClientSecret = clientSecret;
		}

		public async Task<List<MailContext>> RecentMailsAsync(DateTime after)
		{
			var result = await _client.Users[TargetEmail].MailFolders["Inbox"].Messages.GetAsync((requestConfiguration) =>
			{
				requestConfiguration.QueryParameters.Select = new string[] { "sender", "subject", "body", "ConversationId", "ReceivedDateTime", "CcRecipients", "toRecipients" };
				requestConfiguration.QueryParameters.Top = TopN;
				requestConfiguration.QueryParameters.Orderby = ["ReceivedDateTime desc"];
				requestConfiguration.QueryParameters.Filter = $"ReceivedDateTime gt {after.ToString("yyyy-MM-dd")}";
				requestConfiguration.Headers.Add("Prefer", "IdType=\"ImmutableId\"");
			});

			var returnItems = new List<MailContext>();
			foreach (var message in result.Value)
			{
				if (message == null || message.Id == null || message.ConversationId == null ||
					message.Subject == null || message.Sender == null ||
					message.ReceivedDateTime == null || message.Body == null)
					continue;
				var newMail = new MailContext()
				{
					MessageID = message.Id,
					ConversationID = message.ConversationId,
					Subject = message.Subject,
					Sender = message.Sender.EmailAddress.Address,
					Recievers = message.ToRecipients == null ? new List<string>() : message.ToRecipients.Select(x => x.EmailAddress.Address).ToList(),
					CCRecievers = message.CcRecipients == null ? new List<string>() : message.CcRecipients.Select(x => x.EmailAddress.Address).ToList(),
					RecievedAt = (DateTime)message.ReceivedDateTime?.UtcDateTime,
					Body = message.Body.Content
				};

				if (!returnItems.Any(x => x.Equals(newMail)))
					returnItems.Add(newMail);

				if (returnItems.Count >= TopN)
					break;
			}

			returnItems = returnItems.OrderByDescending(x => x.RecievedAt).ToList();

			return returnItems;
		}

		public async Task SendAsync(MailContext mail)
		{
			var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
			{
				Message = new Message
				{
					Subject = mail.Subject,
					Body = new ItemBody
					{
						ContentType = BodyType.Text,
						Content = mail.Body,
					},
					ToRecipients = mail.Recievers.Select(x => EmailToRecipient(x)).ToList(),
					CcRecipients = mail.CCRecievers.Select(x => EmailToRecipient(x)).ToList(),
					ConversationId = mail.ConversationID
				},
				SaveToSentItems = false,
			};
			await _client.Users[mail.Sender.ToString()].SendMail.PostAsync(requestBody);
		}

		public async Task ReplyAsync(MailContext mail, string messageID)
		{
			var requestBody = new Microsoft.Graph.Users.Item.Messages.Item.ReplyAll.ReplyAllPostRequestBody
			{
				Comment = mail.Body,
			};
			await _client.Users[mail.Sender.ToString()].Messages[messageID].ReplyAll.PostAsync(requestBody);
		}

		private Recipient EmailToRecipient(string email) => new Recipient() { EmailAddress = new EmailAddress() { Address = email } };
	}
}
