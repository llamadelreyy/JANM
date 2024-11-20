using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Services
{
    public class EmailHelper : ControllerBase
    {
        private readonly PBTProDbContext _dbcontext;
        private readonly IEmailSender _emailSender;
        private readonly int _maxRetry = 5;

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
                var template = await _dbcontext.config_email_templates.Where(x => x.template_code == code).Select(x => new { x.template_subject, x.template_content }).AsNoTracking().FirstOrDefaultAsync();
                if (template != null)
                {
                    //Apply template from database if available
                    if (!string.IsNullOrWhiteSpace(template.template_subject))
                    {
                        result.subject = template.template_subject;
                    }

                    if (!string.IsNullOrWhiteSpace(template.template_content))
                    {
                        result.body = template.template_content;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(result.body))
                    {
                        var createdBy = await getDefRunUser();
                        config_email_template data = new config_email_template { 
                            template_code = code, 
                            template_subject = result.subject, 
                            template_content = result.body, 
                            created_by = createdBy, 
                            created_date = DateTime.Now 
                        };

                        _dbcontext.config_email_templates.Add(data);
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

        public async Task<notification_email_queue> QueueEmail(string subject, string body, string toEmail)
        {
            notification_email_queue result = new notification_email_queue();
            try
            {
                var createdBy = await getDefRunUser();

                notification_email_queue EmailQueue = new notification_email_queue()
                {
                    queue_recipient = toEmail,
                    queue_subject = subject,
                    queue_content = body,
                    queue_status = "New",
                    created_by = createdBy,
                    created_date = DateTime.Now
                };

                _dbcontext.notification_email_queues.Add(EmailQueue);
                await _dbcontext.SaveChangesAsync();

                result = EmailQueue;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<notification_email_queue> ForceProcessQueue(notification_email_queue queue)
        {
            queue.queue_cnt_retry = queue.queue_cnt_retry + 1;

            notification_email_queue result = new notification_email_queue();
            try
            {
                var createdBy = await getDefRunUser();

                EmailSenderRs emailRs = await _emailSender.SendEmail(queue.queue_subject, queue.queue_content, queue.queue_recipient);

                queue.queue_status = emailRs.Status;
                queue.queue_remark = emailRs.Remars;
                queue.update_date= DateTime.Now;
                queue.updated_by = createdBy;

                _dbcontext.notification_email_queues.Update(queue);
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
        protected async Task<int> getDefRunUser()
        {
            var result = 0;//"System";
            /*
            try
            {
                result = User?.Identity?.Name;
            }
            catch (Exception ex)
            {
                result = "System";
            }
            */
            return result;
        }
        #endregion
    }
}
