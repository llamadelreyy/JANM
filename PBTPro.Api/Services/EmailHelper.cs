using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Security.Claims;

namespace PBTPro.Api.Services
{
    public class EmailHelper : ControllerBase
    {
        private readonly ILogger<EmailHelper> _logger;
        private readonly PBTProDbContext _dbcontext;
        private readonly IEmailSender _emailSender;
        private readonly string _feature = "EMAILHELPER";
        private readonly int _maxRetry = 5;

        public EmailHelper(PBTProDbContext dbcontext, IEmailSender emailSender)
        {
            _dbcontext = dbcontext;
            _emailSender = emailSender;
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EmailHelper>();
        }

        public async Task<EmailContent> getEmailContent(string code, string[]? param, EmailContent? defTemplate)
        {
            var result = new EmailContent();

            //Apply default template if available
            result.subject = defTemplate?.subject;
            result.body = defTemplate?.body;

            try
            {
                var template = await _dbcontext.app_email_tmpls.Where(x => x.tmpl_code == code).Select(x => new { x.tmpl_subject, x.tmpl_content }).AsNoTracking().FirstOrDefaultAsync();
                if (template != null)
                {
                    //Apply template from database if available
                    if (!string.IsNullOrWhiteSpace(template.tmpl_subject))
                    {
                        result.subject = template.tmpl_subject;
                    }

                    if (!string.IsNullOrWhiteSpace(template.tmpl_content))
                    {
                        result.body = template.tmpl_content;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(result.body))
                    {
                        var createdBy = await getDefRunUserId();
                        app_email_tmpl data = new app_email_tmpl {
                            tmpl_code = code,
                            tmpl_subject = result.subject,
                            tmpl_content = result.body, 
                            creator_id = createdBy, 
                            created_at = DateTime.Now 
                        };

                        _dbcontext.app_email_tmpls.Add(data);
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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }
            return result;
        }

        public async Task<trn_email_queue> QueueEmail(string subject, string body, string toEmail)
        {
            trn_email_queue result = new trn_email_queue();
            try
            {
                var createdBy = await getDefRunUserId();

                trn_email_queue EmailQueue = new trn_email_queue()
                {
                    queue_recipient = toEmail,
                    queue_subject = subject,
                    queue_content = body,
                    queue_status = "New",
                    creator_id = createdBy,
                    created_at = DateTime.Now
                };

                _dbcontext.trn_email_queues.Add(EmailQueue);
                await _dbcontext.SaveChangesAsync();

                result = EmailQueue;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<EmailSenderRs> ForceProcessQueue(trn_email_queue queue)
        {
            queue.cnt_retry = queue.cnt_retry + 1;

            EmailSenderRs result = new EmailSenderRs();
            try
            {
                var createdBy = await getDefRunUserId();

                EmailSenderRs emailRs = await _emailSender.SendEmail(queue.queue_subject, queue.queue_content, queue.queue_recipient);

                queue.queue_status = emailRs.Status;
                queue.queue_remark = emailRs.Remars;
                queue.modified_at= DateTime.Now;
                queue.modifier_id = createdBy;

                if (emailRs.isSuccess)
                {
                    queue.date_sent = DateTime.Now;
                    if (await ArchiveQueue(queue))
                    {
                        _dbcontext.trn_email_queues.Remove(queue);
                    }
                }
                else if (emailRs.isSuccess != true && queue.cnt_retry >= _maxRetry)
                {
                    if (await ArchiveQueue(queue))
                    {
                        _dbcontext.trn_email_queues.Remove(queue);
                    }
                }
                else
                {
                    _dbcontext.trn_email_queues.Update(queue);
                }

                await _dbcontext.SaveChangesAsync();

                result = emailRs;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return result;
            }
        }

        #region Private Logic
        protected async Task<string> getDefRunUser()
        {
            string? result = "system";
            try
            {
                result = User?.Identity?.Name;
            }
            catch (Exception ex)
            {
                result = "system";
            }
            return result ?? "system";
        }

        protected async Task<int> getDefRunUserId()
        {
            int result = 0;
            try
            {
                result = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }


        private async Task<bool> ArchiveQueue(trn_email_queue queue)
        {
            bool result = true;
            try
            {
                using (PBTProDbContext tmpDBcontext = new PBTProDbContext())
                {
                    var history = new his_email_history
                    {
                        hist_recipient = queue.queue_recipient,
                        hist_subject = queue.queue_subject,
                        hist_content = queue.queue_content,
                        hist_status = queue.queue_status,
                        hist_remark = queue.queue_remark,
                        date_sent = queue.date_sent,
                        cnt_retry = queue.cnt_retry,
                        is_deleted = queue.is_deleted,
                        creator_id = queue.creator_id,
                        created_at = queue.created_at,
                        modifier_id = queue.modifier_id,
                        modified_at = queue.modified_at
                    };

                    tmpDBcontext.his_email_histories.Add(history);
                    await tmpDBcontext.SaveChangesAsync(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                result = false;
            }

            return result;
        }
        #endregion
    }
}
