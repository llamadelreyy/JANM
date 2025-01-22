using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using System;
using System.Net.Mail;

namespace PBTPro.Api.Services
{
    public interface IEmailSender
    {
        Task<EmailConfiguration> GetConfiguration(EmailConfiguration? DefaultConfiguration);
        Task<EmailSenderRs> SendEmail(string subject, string body, string toEmail, Attachment? doc = null);
        Task<EmailSenderRs> SendEmailMultiDoc(string subject, string body, string toEmail, List<Attachment>? docs = null);
        Task<EmailSenderRs> SendAbundentEmail(string subject, string body, string toEmail, string toCc, string toBcc, List<Attachment>? docs = null);
    }

    public class EmailSender : IEmailSender
    {
        private readonly PBTProDbContext _dbContext;
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailConfiguration _emailConfig;
        private string _feature = "EmailSender";

        public EmailSender(PBTProDbContext dbContext, EmailConfiguration emailConfig)
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EmailSender>();
            _dbContext = dbContext;
            _emailConfig = GetConfiguration(emailConfig).GetAwaiter().GetResult();
        }

        public async Task<EmailConfiguration> GetConfiguration(EmailConfiguration? DefaultConfiguration)
        {
            EmailConfiguration Result = new EmailConfiguration();
            try
            {
                var Configs = await _dbContext.app_system_params.Where(x => x.param_group == "Emailer").AsNoTracking().ToListAsync();
                if (Configs == null || Configs.Count == 0)
                {
                    Result = DefaultConfiguration ?? new EmailConfiguration();
                    return Result;
                }
                foreach (var Config in Configs)
                {
                    if (Config.param_name == "From")
                    {
                        Result.From = Config.param_value;
                        continue;
                    }

                    if (Config.param_name == "SmtpServer")
                    {
                        Result.SmtpServer = Config.param_value;
                        continue;

                    }

                    if (Config.param_name == "Port")
                    {
                        int Port;

                        if (int.TryParse(Config.param_value, out Port))
                        {
                            Result.Port = Port;
                        }
                        continue;
                    }

                    if (Config.param_name == "EnableSsl")
                    {
                        Result.EnableSsl = false;
                        bool EnableSsl;

                        if (Boolean.TryParse(Config.param_value, out EnableSsl))
                        {
                            Result.EnableSsl = EnableSsl;
                        }
                        continue;
                    }

                    if (Config.param_name == "UserName")
                    {
                        Result.UserName = Config.param_value;
                        continue;

                    }

                    if (Config.param_name == "Password")
                    {
                        Result.Password = Config.param_value;
                        continue;

                    }
                }
            }
            catch (Exception ex)
            {
                Result = DefaultConfiguration ?? new EmailConfiguration();
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Result;
        }

        public async Task<EmailSenderRs> SendEmail(string subject, string body, string toEmail, Attachment? doc = null)
        {
            EmailSenderRs Result = new EmailSenderRs();
            Result.isSuccess = false;
            Result.Status = "Failed";

            try
            {
                using MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailConfig.From);
                mailMessage.To.Add(new MailAddress(toEmail));
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                if (doc != null) mailMessage.Attachments.Add(doc);

                Result = await InternalSendEmail(mailMessage);
            }
            catch (Exception ex)
            {
                Result.Remars = ex.Message;
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Result;
        }

        public async Task<EmailSenderRs> SendEmailMultiDoc(string subject, string body, string toEmail, List<Attachment>? docs = null)
        {
            EmailSenderRs Result = new EmailSenderRs();
            Result.isSuccess = false;
            Result.Status = "Failed";

            try
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
                Result = await InternalSendEmail(mailMessage);
            }
            catch (Exception ex)
            {
                Result.Remars = ex.Message;
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Result;
        }

        public async Task<EmailSenderRs> SendAbundentEmail(string subject, string body, string toEmail, string toCc, string toBcc, List<Attachment>? docs = null)
        {
            EmailSenderRs Result = new EmailSenderRs();
            Result.isSuccess = false;
            Result.Status = "Failed";

            try
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

                Result = await InternalSendEmail(mailMessage);
            }
            catch (Exception ex)
            {
                Result.Remars = ex.Message;
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Result;
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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Result;
        }
    }
}
