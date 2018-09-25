using System.Collections.Generic;
using System.Threading.Tasks;
using Daarto.Models;

namespace Daarto.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(IEnumerable<EmailRecipient> recipients, string subject, string body);
    }
}
