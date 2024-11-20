using PBTPro.DAL.Models.CommonServices;
using System.Net.Mail;

namespace PBTPro.Api.Services
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public interface IEmailSender
    {
        Task<EmailSenderRs> SendEmail(string subject, string body, string toEmail, Attachment? doc = null);
        Task<EmailSenderRs> SendEmailMultiDoc(string subject, string body, string toEmail, List<Attachment>? docs = null);
        Task<EmailSenderRs> SendAbundentEmail(string subject, string body, string toEmail, string toCc, string toBcc, List<Attachment>? docs = null);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<EmailSenderRs> SendEmail(string subject, string body, string toEmail, Attachment? doc = null)
        {
            using MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailConfig.From);
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            if (doc != null) mailMessage.Attachments.Add(doc);

            return await InternalSendEmail(mailMessage);
        }

        public async Task<EmailSenderRs> SendEmailMultiDoc(string subject, string body, string toEmail, List<Attachment>? docs = null)
        {
            using MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailConfig.From);
            var totalUserTo = toEmail.Split(';', ',').ToList();
            if (totalUserTo.Count > 0)
            {
                foreach (var userTo in totalUserTo)
                {
                    if (IsValidEmail(userTo))
                    {
                        mailMessage.To.Add(new MailAddress(userTo));
                    }
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            if (docs != null)
            {
                if (docs.Count > 0)
                {
                    foreach (var doc in docs)
                    {
                        mailMessage.Attachments.Add(doc);
                    }
                }
            }
            return await InternalSendEmail(mailMessage);
        }

        public async Task<EmailSenderRs> SendAbundentEmail(string subject, string body, string toEmail, string toCc, string toBcc, List<Attachment>? docs = null)
        {
            using MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailConfig.From);

            if (toEmail != null)
            {
                var totalUserTo = toEmail.Split(';', ',').ToList();
                if (totalUserTo.Count > 0)
                {
                    foreach (var userTo in totalUserTo)
                    {
                        if (IsValidEmail(userTo))
                        {
                            mailMessage.To.Add(userTo);
                        }

                    }

                }
            }

            if (toCc != null)
            {
                var totalUserToCc = toCc.Split(';', ',').ToList();
                if (totalUserToCc.Count > 0)
                {
                    foreach (var userCc in totalUserToCc)
                    {
                        if (IsValidEmail(userCc))
                        {
                            mailMessage.CC.Add(userCc);
                        }

                    }

                }
            }

            if (toBcc != null)
            {
                var totalUserToBcc = toBcc.Split(';', ',').ToList();
                if (totalUserToBcc.Count > 0)
                {
                    foreach (var userBcc in totalUserToBcc)
                    {
                        if (IsValidEmail(userBcc))
                        {
                            mailMessage.Bcc.Add(userBcc);
                        }

                    }
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            if (docs != null)
            {
                if (docs.Count > 0)
                {
                    foreach (var doc in docs)
                    {
                        mailMessage.Attachments.Add(doc);
                    }
                }
            }

            return await InternalSendEmail(mailMessage);
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task<EmailSenderRs> InternalSendEmail(MailMessage mailMessage)
        {
            EmailSenderRs Result = new EmailSenderRs();
            Result.isSuccess = false;
            Result.Status = "Failed";

            using var client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
            client.Host = _emailConfig.SmtpServer;
            client.Port = _emailConfig.Port;
            client.EnableSsl = _emailConfig.EnableSsl;

            try
            {
                client.Send(mailMessage);
                Result.isSuccess = true;
                Result.Status = "Successful";
            }
            catch (Exception ex)
            {
                Result.Remars = ex.Message;
            }

            return Result;
        }
    }
}
