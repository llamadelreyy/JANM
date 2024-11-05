using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;

namespace PBTPro.Api.Services
{
    public class EmailHelper : ControllerBase
    {
        private readonly PBTProDbContext _dbcontext;
        private readonly IEmailSender _emailSender;

        public EmailHelper(PBTProDbContext dbcontext, IEmailSender emailSender)
        {
            _dbcontext = dbcontext;
            _emailSender = emailSender;
        }

        public async Task<EmailContent> getEmailContent(string code, string[]? param, EmailContent? defTemplate)
        {
            var result = new EmailContent();

            //Apply default template if available
            result.subject = defTemplate?.subject;
            result.body = defTemplate?.body;

            try
            {
                var template = await _dbcontext.AppEmailTemplates.Where(x => x.Code == code).Select(x => new { x.Subject, x.Content }).AsNoTracking().FirstOrDefaultAsync();
                if (template != null)
                {
                    //Apply template from database if available
                    if (!string.IsNullOrWhiteSpace(template.Subject))
                    {
                        result.subject = template.Subject;
                    }

                    if (!string.IsNullOrWhiteSpace(template.Content))
                    {
                        result.body = template.Content;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(result.body))
                    {
                        string createdBy = await getDefRunUser();
                        AppEmailTemplate data = new AppEmailTemplate { Code = code, Subject = result.subject, Content = result.body, CreatedBy = createdBy, CreatedDtm = DateTime.Now };
                        _dbcontext.AppEmailTemplates.Add(data);
                        await _dbcontext.SaveChangesAsync();
                    }
                }

                //Apply value to the threshold
                if (param?.Length > 0)
                {
                    int k = 0;
                    foreach (string par in param)
                    {
                        result.subject = result.subject.Replace($"[{k}]", par);
                        result.body = result.body.Replace($"[{k}]", par);
                        k++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return result;
        }

        public async Task<AppEmailQueue> QueueEmail(string subject, string body, string toEmail)
        {
            AppEmailQueue result = new AppEmailQueue();
            try
            {
                string createdBy = await getDefRunUser();

                AppEmailQueue EmailQueue = new AppEmailQueue()
                {
                    ToEmail = toEmail,
                    Subject = subject,
                    Content = body,
                    Status = "New",
                    CreatedBy = createdBy,
                    CreatedDtm = DateTime.Now
                };

                _dbcontext.AppEmailQueues.Add(EmailQueue);
                await _dbcontext.SaveChangesAsync();

                result = EmailQueue;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<AppEmailQueue> ForceProcessQueue(AppEmailQueue queue)
        {
            AppEmailQueue result = new AppEmailQueue();
            try
            {
                string createdBy = await getDefRunUser();

                EmailSenderRs emailRs = await _emailSender.SendEmail(queue.Subject, queue.Content, queue.ToEmail);

                queue.Status = emailRs.Status;
                queue.Remark = emailRs.Remars;
                queue.ModifiedDtm = DateTime.Now;
                queue.ModifiedBy = createdBy;

                _dbcontext.AppEmailQueues.Update(queue);
                await _dbcontext.SaveChangesAsync();

                result = queue;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        #region Private Logic
        protected async Task<string> getDefRunUser()
        {
            var result = "System";
            try
            {
                result = User?.Identity?.Name;
            }
            catch (Exception ex)
            {
                result = "System";
            }
            return result;
        }
        #endregion
    }
}
