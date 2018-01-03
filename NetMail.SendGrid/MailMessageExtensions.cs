using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using SendGridEmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using SendGridEmailAttachment = SendGrid.Helpers.Mail.Attachment;
using SendGridMessage = SendGrid.Helpers.Mail.SendGridMessage;

namespace NetMail.SendGrid
{
    // https://github.com/sendgrid/sendgrid-csharp/issues/266

    public static partial class MailMessageExtensions
    {
        public static SendGridEmailAddress GetSendGridAddress(this MailAddress address)
        {
            return String.IsNullOrWhiteSpace(address.DisplayName) ?
                new SendGridEmailAddress(address.Address) :
                new SendGridEmailAddress(address.Address, address.DisplayName.Replace(",", "").Replace(";", ""));
        }

        public static SendGridEmailAttachment GetSendGridAttachment(this Attachment attachment)
        {
            using (var stream = new MemoryStream())
            {
                attachment.ContentStream.CopyTo(stream);
                return new SendGridEmailAttachment()
                {
                    Disposition = "attachment",
                    Type = attachment.ContentType.MediaType,
                    Filename = attachment.Name,
                    ContentId = attachment.ContentId,
                    Content = Convert.ToBase64String(stream.ToArray())
                };
            }
        }

        public static SendGridMessage GetSendGridMessage(this MailMessage message)
        {
            var sendgridMessage = new SendGridMessage
            {
                From = GetSendGridAddress(message.From)
            };

            if (message.ReplyToList.Any())
            {
                sendgridMessage.ReplyTo = message.ReplyToList.First().GetSendGridAddress();
            }

            if (message.To.Any())
            {
                var tos = message.To.Select(x => x.GetSendGridAddress()).ToList();
                sendgridMessage.AddTos(tos);
            }

            if (message.CC.Any())
            {
                var cc = message.CC.Select(x => x.GetSendGridAddress()).ToList();
                sendgridMessage.AddCcs(cc);
            }

            if (message.Bcc.Any())
            {
                var bcc = message.Bcc.Select(x => x.GetSendGridAddress()).ToList();
                sendgridMessage.AddBccs(bcc);
            }

            if (!string.IsNullOrWhiteSpace(message.Subject))
            {
                sendgridMessage.Subject = message.Subject;
            }

            if (!string.IsNullOrWhiteSpace(message.Body))
            {
                var content = message.Body;

                if (message.IsBodyHtml)
                {

                    if (content.StartsWith("<html"))
                    {
                        content = message.Body;
                    }
                    else
                    {
                        content = $"<html><body>{message.Body}</body></html>";
                    }

                    sendgridMessage.AddContent("text/html", content);
                }
                else
                {
                    sendgridMessage.AddContent("text/plain", content);
                }
            }

            if (message.Attachments.Any())
            {
                sendgridMessage.Attachments = new List<SendGridEmailAttachment>();
                sendgridMessage.Attachments.AddRange(message.Attachments.Select(x => GetSendGridAttachment(x)));
            }

            return sendgridMessage;
        }
    }
}