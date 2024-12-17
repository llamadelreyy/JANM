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

        public async Task<EmailSenderRs> ForceProcessQueue(notification_email_queue queue)
        {
            queue.queue_cnt_retry = queue.queue_cnt_retry + 1;

            EmailSenderRs result = new EmailSenderRs();
            try
            {
                var createdBy = await getDefRunUser();

                EmailSenderRs emailRs = await _emailSender.SendEmail(queue.queue_subject, queue.queue_content, queue.queue_recipient);

                queue.queue_status = emailRs.Status;
                queue.queue_remark = emailRs.Remars;
                queue.update_date= DateTime.Now;
                queue.updated_by = createdBy;

                if (emailRs.isSuccess)
                {
                    queue.queue_date_sent = DateTime.Now;
                    if (await ArchiveQueue(queue))
                    {
                        _dbcontext.notification_email_queues.Remove(queue);
                    }
                }
                else if (emailRs.isSuccess != true && queue.queue_cnt_retry >= _maxRetry)
                {
                    if (await ArchiveQueue(queue))
                    {
                        _dbcontext.notification_email_queues.Remove(queue);
                    }
                }
                else
                {
                    _dbcontext.notification_email_queues.Update(queue);
                }

                await _dbcontext.SaveChangesAsync();

                result = emailRs;
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

        private async Task<bool> ArchiveQueue(notification_email_queue queue)
        {
            bool result = true;
            try
            {
                using (PBTProDbContext tmpDBcontext = new PBTProDbContext())
                {
                    var history = new notification_email_history
                    {
                        history_recipient = queue.queue_recipient,
                        history_subject = queue.queue_subject,
                        history_content = queue.queue_content,
                        history_status = queue.queue_status,
                        history_remark = queue.queue_remark,
                        history_date_sent = queue.queue_date_sent,
                        history_cnt_retry = queue.queue_cnt_retry,
                        active_flag = queue.active_flag,
                        created_by = queue.created_by,
                        created_date = queue.created_date,
                        updated_by = queue.updated_by,
                        update_date = queue.update_date
                    };

                    tmpDBcontext.notification_email_histories.Add(history);
                    await tmpDBcontext.SaveChangesAsync(false);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
        #endregion
    }
}
