using Daarto.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daarto.Services.Abstract
{
    public interface IEmailService
    {
        Task SendMailAsync(IEnumerable<EmailRecipient> recipients, string subject, string body);
    }
}