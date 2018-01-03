using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetMail
{
    public abstract class EmailProcessorBase : IEmailProcessor
    {
        public abstract Task SendAsync(MailMessage message);

        protected virtual MailMessage NewMessage()
        {
            return new MailMessage();
        }

        public async Task SendAsync(Action<MailMessage> messageHandler)
        {
            using (var message = NewMessage())
            {
                messageHandler(message);
                await SendAsync(message);
            }
        }

        #region IDisposable Support
        int _disposeState = 0; // To detect redundant calls

        protected abstract void OnDisposing(bool disposing);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposeState == 0 && Interlocked.CompareExchange(ref _disposeState, 1, 0) == 0)
            {
                OnDisposing(disposing);
            }
        }
        ~EmailProcessorBase()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
