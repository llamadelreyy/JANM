using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace PBTPro.DAL.Models.CommonServices
{
    public class Emailer
    {
        public string subject { get; set; }
        public string body { get; set; }
        public List<string>? toEmail { get; set; }
        public string? toCc { get; set; }
        public string? toBcc { get; set; }
        public List<Attachment>? docs { get; set; }
    }

    public class EmailContent
    {
        public string subject { get; set; }
        public string body { get; set; }
    }

    public class EmailSenderRs
    {
        public bool isSuccess { get; set; }
        public string Status { get; set; }
        public string Remars { get; set; }
    }

    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class EmailConfigurationTestMail
    {
        [Required(ErrorMessage = "Ruangan Email Penerima diperlukan.")]
        [EmailAddress(ErrorMessage = "Format email penerima tidak sah.")]
        public string Receipient { get; set; }
    }
}
