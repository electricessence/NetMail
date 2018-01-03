using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGridClient = SendGrid.SendGridClient;
using SendGridMessage = SendGrid.Helpers.Mail.SendGridMessage;

namespace NetMail.SendGrid
{
    public class SendGridProcessor : EmailProcessorBase
    {
        SendGridClient _client;

        public SendGridProcessor(SendGridClient client)
        {
            _client = client;
        }

        public SendGridProcessor(Func<SendGridClient> clientFactory) : this(clientFactory())
        {
        }

        public override Task SendAsync(MailMessage message)
        {
            return SendAsync(message.GetSendGridMessage());
        }

        public Task SendAsync(SendGridMessage message)
        {
            return _client.SendEmailAsync(message);
        }

        protected override void OnDisposing(bool disposing)
        {
            _client = null;
        }
    }
}
