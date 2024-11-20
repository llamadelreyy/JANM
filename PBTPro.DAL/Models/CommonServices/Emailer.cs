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
}
