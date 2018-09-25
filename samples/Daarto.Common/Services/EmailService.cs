using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daarto.Abstractions;
using Daarto.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Daarto.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly MimeMessage _emailMessage;

        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;

            _emailMessage = new MimeMessage
            {
                From = { new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.From) }
            };
        }

        public async Task SendAsync(IEnumerable<EmailRecipient> recipients, string subject, string body)
        {
            _emailMessage.Subject = subject;
            _emailMessage.Body = new TextPart("html") { Text = body };
            _emailMessage.To.AddRange(recipients.Select(e => new MailboxAddress(e.Name, e.EmailAddress)));

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.LocalDomain = _smtpSettings.LocalDomain;
                await smtpClient.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.None).ConfigureAwait(false);
                await smtpClient.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password).ConfigureAwait(false);
                await smtpClient.SendAsync(_emailMessage).ConfigureAwait(false);
                await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
