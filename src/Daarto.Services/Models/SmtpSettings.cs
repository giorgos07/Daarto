namespace Daarto.Services.Models
{
    public class SmtpSettings
    {
        public string From { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string SenderName { get; set; }
        public string LocalDomain { get; set; }
    }
}