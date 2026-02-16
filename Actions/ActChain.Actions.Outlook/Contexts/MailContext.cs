using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace ActChain.Models.Contexts
{
	[JsonDerivedType(typeof(MailContext), typeDiscriminator: nameof(MailContext))]
	public class MailContext : IActionContext
	{
		public string MessageID { get; set; }
		public string ConversationID { get; set; }
		[Required]
		public string Subject { get; set; }
		[Required]
		public string Sender { get; set; }
		[Required]
		public List<string> Recievers { get; set; }
		public DateTime RecievedAt { get; set; }
		[Required]
		public string Body { get; set; }
		public List<string> CCRecievers { get; set; }

		public MailContext()
		{
			MessageID = "";
			ConversationID = "";
			Subject = "Subject";
			Sender = "none@none.com";
			Recievers = new List<string>() { "none@none.com" };
			RecievedAt = DateTime.Now;
			Body = "Empty";
			CCRecievers = new List<string>();
		}

		public MailContext(MailContext other)
		{
			MessageID = other.MessageID;
			ConversationID = other.ConversationID;
			Subject = other.Subject;
			Sender = other.Sender;
			Recievers = other.Recievers;
			RecievedAt = other.RecievedAt;
			Body = other.Body;
			CCRecievers = other.CCRecievers;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(MessageID, ConversationID, Subject, Sender, Recievers, RecievedAt, Body, CCRecievers);
		}

		public override bool Equals(object? obj)
		{
			if (obj is MailContext other)
			{
				if (Subject != other.Subject) return false;
				if (ConversationID != other.ConversationID) return false;
				if (MessageID != other.MessageID) return false;
				if (Sender != other.Sender) return false;
				if (Recievers.Count != other.Recievers.Count) return false;
				for (int i = 0; i < Recievers.Count; i++)
					if (Recievers[i] != other.Recievers[i])
						return false;
				if (RecievedAt.Ticks != other.RecievedAt.Ticks) return false;
				if (Body != other.Body) return false;
				if (CCRecievers.Count != other.CCRecievers.Count) return false;
				for (int i = 0; i < CCRecievers.Count; i++)
					if (CCRecievers[i] != other.CCRecievers[i])
						return false;
				return true;
			}
			return false;
		}

		public string GetContent()
		{
			var sb = new StringBuilder();

			sb.AppendLine(Subject);
			sb.AppendLine(Sender.ToString());
			sb.AppendLine(string.Join(',', Recievers));
			sb.AppendLine(RecievedAt.ToString());
			sb.AppendLine(string.Join(',', CCRecievers));
			sb.AppendLine(GetBodyAsPlaintext());

			return sb.ToString();
		}

		public Dictionary<string, string> GetContextValues()
		{
			var newDict = new Dictionary<string, string>();
			newDict.Add("$type", nameof(MailContext));
			newDict.Add("subject", Subject);
			newDict.Add("sender", Sender);
			newDict.Add("reciever", string.Join(',', Recievers));
			newDict.Add("ccreciever", string.Join(',', CCRecievers));
			newDict.Add("body", Body);
			newDict.Add("conversationid", ConversationID);
			newDict.Add("messageid", MessageID);
			return newDict;
		}

		public IActionContext Clone() => new MailContext(this);

		public string GetBodyAsPlaintext() => HTMLHelper.ConvertToPlainText(Body);

		private static class HTMLHelper
		{
			/// <summary>
			/// Converts HTML to plain text / strips tags.
			/// </summary>
			/// <param name="html">The HTML.</param>
			/// <returns></returns>
			public static string ConvertToPlainText(string html)
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(html);

				StringWriter sw = new StringWriter();
				ConvertTo(doc.DocumentNode, sw);
				sw.Flush();
				return sw.ToString();
			}


			/// <summary>
			/// Count the words.
			/// The content has to be converted to plain text before (using ConvertToPlainText).
			/// </summary>
			/// <param name="plainText">The plain text.</param>
			/// <returns></returns>
			public static int CountWords(string plainText)
			{
				return !string.IsNullOrEmpty(plainText) ? plainText.Split(' ', '\n').Length : 0;
			}


			public static string Cut(string text, int length)
			{
				if (!string.IsNullOrEmpty(text) && text.Length > length)
				{
					text = text.Substring(0, length - 4) + " ...";
				}
				return text;
			}


			private static void ConvertContentTo(HtmlNode node, TextWriter outText)
			{
				foreach (HtmlNode subnode in node.ChildNodes)
				{
					ConvertTo(subnode, outText);
				}
			}


			private static void ConvertTo(HtmlNode node, TextWriter outText)
			{
				string html;
				switch (node.NodeType)
				{
					case HtmlNodeType.Comment:
						// don't output comments
						break;

					case HtmlNodeType.Document:
						ConvertContentTo(node, outText);
						break;

					case HtmlNodeType.Text:
						// script and style must not be output
						string parentName = node.ParentNode.Name;
						if (parentName == "script" || parentName == "style")
							break;

						// get text
						html = ((HtmlTextNode)node).Text;

						// is it in fact a special closing node output as text?
						if (HtmlNode.IsOverlappedClosingElement(html))
							break;

						// check the text is meaningful and not a bunch of whitespaces
						if (html.Trim().Length > 0)
						{
							outText.Write(HtmlEntity.DeEntitize(html));
						}
						break;

					case HtmlNodeType.Element:
						switch (node.Name)
						{
							case "p":
								// treat paragraphs as crlf
								outText.Write("\r\n");
								break;
							case "br":
								outText.Write("\r\n");
								break;
						}

						if (node.HasChildNodes)
						{
							ConvertContentTo(node, outText);
						}
						break;
				}
			}
		}
	}
}
