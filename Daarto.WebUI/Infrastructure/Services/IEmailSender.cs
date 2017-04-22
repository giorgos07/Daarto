using System.Threading.Tasks;

namespace Daarto.WebUI.Infrastructure.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}