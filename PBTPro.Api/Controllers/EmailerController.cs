using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailerController : IBaseController
    {
        private readonly ILogger<EmailerController> _logger;
        private readonly PBTProBkgdSM _bkgdSM;
        private readonly IEmailSender _emailSender;
        private readonly string _feature = "EMAILER";
        private readonly int _maxRetry = 5;

        public EmailerController(PBTProDbContext dbContext, ILogger<EmailerController> logger, IEmailSender emailSender, PBTProBkgdSM bkgdSM) : base(dbContext)
        {
            _logger = logger;
            _emailSender = emailSender;
            _bkgdSM = bkgdSM;
        }

        [HttpGet]
        [Route("GetEmailQueue")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmailQueue(string? ListType)
        {
            try
            {
                List<notification_email_queue> QueueList = new List<notification_email_queue>();

                IQueryable<notification_email_queue> InitQuery = _dbContext.notification_email_queues;
                if (!string.IsNullOrWhiteSpace(ListType))
                {
                    switch (ListType)
                    {
                        case "Failed":
                            InitQuery = InitQuery.Where(x => x.queue_status == "Failed");
                            break;
                        case "Success":
                            InitQuery = InitQuery.Where(x => x.queue_status == "Success");
                            break;
                        case "InQueue":
                            InitQuery = InitQuery.Where(x => x.queue_status == "New");
                            break;
                        default:
                            return Error("", "List Type is not supported");
                    }
                }

                QueueList = await InitQuery.OrderByDescending(x => x.created_date).AsNoTracking().ToListAsync();

                if (QueueList.Count > 0)
                {
                    return Ok(QueueList, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }
                else
                {
                    return NoContent(SystemMesg(_feature, "EMPTY_RECORD", MessageTypeEnum.Error, string.Format("Tiada rekod dijumpai")));
                }
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        [HttpGet]
        [Route("GetEmailTemplate")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmailTemplate(string Code)
        {
            try
            {
                EmailContent template = await _dbContext.config_email_templates.Where(x => x.template_code == Code).Select(x => new EmailContent { subject = x.template_subject, body = x.template_content }).AsNoTracking().FirstOrDefaultAsync();

                if (template != null)
                {
                    return Ok(template, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data template berjaya dijana")));
                }
                else
                {
                    return NoContent(SystemMesg(_feature, "EMPTY_RECORD", MessageTypeEnum.Error, string.Format("Tiada rekod dijumpai")));
                }
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("ProcessEmailQueue")]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessEmailQueue()
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                string ServiceName = "EmailSender";

                var isExists = _bkgdSM.GetBackgroundServiceQueue(ServiceName);

                if (isExists == null)
                {
                    var dbOptions = _dbContext.GetService<IDbContextServices>().ContextOptions;

                    _bkgdSM.StartBackgroundService(ServiceName);
                    _bkgdSM.EnqueueWorkItem(ServiceName, async token =>
                    {
                        try
                        {
                            using (PBTProDbContext _dbcontext = new PBTProDbContext((DbContextOptions<PBTProDbContext>)dbOptions))
                            {
                                List<notification_email_queue> QueueLists = await _dbcontext.notification_email_queues.Where(x => x.queue_status != "Successful" && x.queue_cnt_retry < _maxRetry).OrderBy(x => x.created_date).ToListAsync();

                                if (QueueLists.Count > 0)
                                {
                                    foreach (var queue in QueueLists)
                                    {
                                        EmailSenderRs emailRs = await _emailSender.SendEmail(queue.queue_subject, queue.queue_content, queue.queue_recipient);

                                        queue.queue_cnt_retry = queue.queue_cnt_retry + 1;
                                        queue.queue_status = emailRs.Status;
                                        queue.queue_remark = emailRs.Remars;
                                        queue.update_date = DateTime.Now;
                                        queue.updated_by = runUserID;

                                        if (emailRs.isSuccess == true)
                                        {
                                            queue.queue_date_sent = DateTime.Now;
                                            if (await ArchiveQueue(queue))
                                            {
                                                _dbcontext.notification_email_queues.Remove(queue);
                                            }
                                        }
                                        else if(emailRs.isSuccess != true && queue.queue_cnt_retry >= _maxRetry)
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
                                        token.ThrowIfCancellationRequested();
                                    }
                                }
                            }
                        }
                        catch (OperationCanceledException OCex)
                        {
                            Console.WriteLine("Service Stoped");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error : " + ex.Message);
                        }

                        try
                        {
                            _bkgdSM.RemoveBackgroundService(ServiceName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error : " + ex.Message);
                        }
                    });
                }

                return Ok("", SystemMesg(_feature, "PROCESS_EMAIL_QUEUE", MessageTypeEnum.Success, string.Format("Berjaya process email yang tersusun")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        #region Private Logic
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
