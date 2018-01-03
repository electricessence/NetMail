using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NetMail
{
    public class SmtpProcessor : EmailProcessorBase
    {
        readonly SmtpClient _client;

        public SmtpProcessor(Func<SmtpClient> clientFactory)
        {
            _client = clientFactory();
        }

        public override Task SendAsync(MailMessage message)
        {
            return _client.SendMailAsync(message);
        }

        protected override void OnDisposing(bool disposing)
        {
            if (disposing) _client?.Dispose();
        }
    }
}
