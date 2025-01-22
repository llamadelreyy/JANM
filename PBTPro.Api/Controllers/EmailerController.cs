using DevExpress.XtraPrinting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Xml.Linq;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
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
        //[Route("GetEmailQueue")]
        public async Task<IActionResult> GetEmailConfig()
        {
            try
            {
                EmailConfiguration Result = await _emailSender.GetConfiguration(null);

                return Ok(Result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveEmailConfig([FromBody] EmailConfiguration InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();

                EmailConfiguration Result = new EmailConfiguration();
                var ConfigNew = new List<app_system_param>();
                var ConfigUpd = new List<app_system_param>();
                var Configs = await _dbContext.app_system_params.Where(x => x.param_group == "Emailer").ToListAsync();

                #region build data to save
                app_system_param? From = Configs.FirstOrDefault(x => x.param_name == "From");
                if (From != null)
                {
                    if (From.param_value != InputModel.From)
                    {
                        From.param_value = InputModel.From;
                        From.modified_at = DateTime.Now;
                        From.modifier_id = runUserID;
                        ConfigUpd.Add(From);
                    }
                }
                else
                {
                    From = new app_system_param {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "From",
                        param_value = InputModel.From,
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(From);
                }

                app_system_param? SmtpServer = Configs.FirstOrDefault(x => x.param_name == "SmtpServer");
                if (SmtpServer != null)
                {
                    if (SmtpServer.param_value != InputModel.SmtpServer)
                    {
                        SmtpServer.param_value = InputModel.SmtpServer;
                        SmtpServer.modified_at = DateTime.Now;
                        SmtpServer.modifier_id = runUserID;
                        ConfigUpd.Add(SmtpServer);
                    }
                }
                else
                {
                    SmtpServer = new app_system_param
                    {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "SmtpServer",
                        param_value = InputModel.SmtpServer,
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(SmtpServer);
                }

                app_system_param? Port = Configs.FirstOrDefault(x => x.param_name == "Port");
                if (Port != null)
                {
                    if (Port.param_value != InputModel.Port.ToString())
                    {
                        Port.param_value = InputModel.Port.ToString();
                        Port.modified_at = DateTime.Now;
                        Port.modifier_id = runUserID;
                        ConfigUpd.Add(Port);
                    }
                }
                else
                {
                    Port = new app_system_param
                    {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "Port",
                        param_value = InputModel.Port.ToString(),
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(Port);
                }

                app_system_param? EnableSsl = Configs.FirstOrDefault(x => x.param_name == "EnableSsl");
                if (EnableSsl != null)
                {
                    if (EnableSsl.param_value != InputModel.EnableSsl.ToString())
                    {
                        EnableSsl.param_value = InputModel.EnableSsl.ToString();
                        EnableSsl.modified_at = DateTime.Now;
                        EnableSsl.modifier_id = runUserID;
                        ConfigUpd.Add(EnableSsl);
                    }
                }
                else
                {
                    EnableSsl = new app_system_param
                    {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "EnableSsl",
                        param_value = InputModel.EnableSsl.ToString(),
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(EnableSsl);
                }

                app_system_param? UserName = Configs.FirstOrDefault(x => x.param_name == "UserName");
                if (UserName != null)
                {
                    if (UserName.param_value != InputModel.UserName)
                    {
                        UserName.param_value = InputModel.UserName;
                        UserName.modified_at = DateTime.Now;
                        UserName.modifier_id = runUserID;
                        ConfigUpd.Add(UserName);
                    }
                }
                else
                {
                    UserName = new app_system_param
                    {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "UserName",
                        param_value = InputModel.UserName,
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(UserName);
                }

                app_system_param? Password = Configs.FirstOrDefault(x => x.param_name == "Password");
                if (Password != null)
                {
                    if (Password.param_value != InputModel.Password)
                    {
                        Password.param_value = InputModel.Password;
                        Password.modified_at = DateTime.Now;
                        Password.modifier_id = runUserID;
                        ConfigUpd.Add(Password);
                    }
                }
                else
                {
                    Password = new app_system_param
                    {
                        app_layer = "APP",
                        param_group = "Emailer",
                        param_name = "Password",
                        param_value = InputModel.Password,
                        created_at = DateTime.Now,
                        creator_id = runUserID,
                        is_deleted = false,
                    };
                    ConfigNew.Add(Password);
                }
                #endregion

                #region Saving Changes
                bool doSaveChanges = false;
                if (ConfigNew.Count > 0) {
                    _dbContext.app_system_params.AddRange(ConfigNew);
                    doSaveChanges = true;
                }

                if (ConfigUpd.Count > 0)
                {
                    _dbContext.app_system_params.UpdateRange(ConfigUpd);
                    doSaveChanges = true;
                }

                if (doSaveChanges == true)
                {
                    await _dbContext.SaveChangesAsync();
                }
                #endregion
                return Ok(Result, SystemMesg(_feature, "SAVE_EMAIL_COMFIG", MessageTypeEnum.Success, string.Format("Berjaya mengemaskini Konfigurasi Email")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestEmailConfig([FromBody] EmailConfigurationTestMail InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                DateTime startedDTM = DateTime.Now;

                string Receipient = InputModel.Receipient;
                //Default Email Template
                EmailContent defaultContent = new EmailContent
                {
                    subject = "Ujian Konfigurasi E-mel PBTPro",
                    body = "Assalamualaikum, <br/><br/>" +
                    "Ini adalah e-mel ujian untuk mengesahkan tetapan konfigurasi sistem e-mel kami. Sila sahkan jika anda telah menerima e-mel ini dengan jayanya.<br/><br/>" +
                    "Terima Kasih.<br/><br/>Salam sejahtera,<br/>Pentadbir PBTPro<br/><br/><i>**Ini adalah mesej automatik, sila jangan balas**</i>",
                };

                string[] param = {};
                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("EMAIL_CONFIG_TESTING", param, defaultContent);

                EmailSenderRs emailRs = await _emailSender.SendEmail(emailContent.subject, emailContent.body, Receipient);

                await ArchiveQueue(new trn_email_queue
                {
                    queue_recipient = Receipient,
                    queue_subject = emailContent.subject,
                    queue_content = emailContent.body,
                    queue_status = emailRs.Status,
                    queue_remark = emailRs.Remars,
                    date_sent = emailRs.isSuccess == true ? DateTime.Now : null,
                    cnt_retry = 1,
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = startedDTM,
                    modifier_id = runUserID,
                    modified_at = DateTime.Now
                });

                List<string> paramRS = new List<string> { Receipient };
                if (emailRs.isSuccess == true)
                {
                    return Ok(emailRs, SystemMesg(_feature, "SENT_TESTING", MessageTypeEnum.Success, string.Format("Berjaya menghantar email ujian konfigurasi ke email [0]"), paramRS));
                }
                else
                {
                    paramRS.Add(emailRs.Remars);
                    return Error(emailRs, SystemMesg(_feature, "SENT_TESTING", MessageTypeEnum.Error, string.Format("Gagal menghantar email ujian konfigurasi ke email [0], Ralat [1]"), paramRS));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetEmailQueue")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmailQueue(string? ListType)
        {
            try
            {
                List<trn_email_queue> QueueList = new List<trn_email_queue>();

                IQueryable<trn_email_queue> InitQuery = _dbContext.trn_email_queues;
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

                QueueList = await InitQuery.OrderByDescending(x => x.created_at).AsNoTracking().ToListAsync();

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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetEmailTemplate")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmailTemplate(string Code)
        {
            try
            {
                EmailContent template = await _dbContext.app_email_tmpls.Where(x => x.tmpl_code == Code).Select(x => new EmailContent { subject = x.tmpl_subject, body = x.tmpl_content }).AsNoTracking().FirstOrDefaultAsync();

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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("ProcessEmailQueue")]
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
                    //var dbOptions = _dbContext.GetService<IDbContextServices>().ContextOptions;

                    _bkgdSM.StartBackgroundService(ServiceName);
                    _bkgdSM.EnqueueWorkItem(ServiceName, async token =>
                    {
                        try
                        {
                            using (PBTProDbContext _dbcontext = new PBTProDbContext())
                            {
                                List<trn_email_queue> QueueLists = await _dbcontext.trn_email_queues.Where(x => x.queue_status != "Successful" && x.cnt_retry < _maxRetry).OrderBy(x => x.created_at).ToListAsync();

                                if (QueueLists.Count > 0)
                                {
                                    foreach (var queue in QueueLists)
                                    {
                                        EmailSenderRs emailRs = await _emailSender.SendEmail(queue.queue_subject, queue.queue_content, queue.queue_recipient);

                                        queue.cnt_retry = queue.cnt_retry + 1;
                                        queue.queue_status = emailRs.Status;
                                        queue.queue_remark = emailRs.Remars;
                                        queue.modified_at = DateTime.Now;
                                        queue.modifier_id = runUserID;

                                        if (emailRs.isSuccess == true)
                                        {
                                            queue.date_sent = DateTime.Now;
                                            if (await ArchiveQueue(queue))
                                            {
                                                _dbcontext.trn_email_queues.Remove(queue);
                                            }
                                        }
                                        else if(emailRs.isSuccess != true && queue.cnt_retry >= _maxRetry)
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
                                        token.ThrowIfCancellationRequested();
                                    }
                                }
                            }
                        }
                        catch (OperationCanceledException OCex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, OCex.Message, OCex.InnerException));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        }

                        try
                        {
                            _bkgdSM.RemoveBackgroundService(ServiceName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        }
                    });
                }

                return Ok("", SystemMesg(_feature, "PROCESS_EMAIL_QUEUE", MessageTypeEnum.Success, string.Format("Berjaya process email yang tersusun")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        #region Private Logic
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
