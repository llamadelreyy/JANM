using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;

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
                List<AppEmailQueue> QueueList = new List<AppEmailQueue>();

                IQueryable<AppEmailQueue> InitQuery = _dbContext.AppEmailQueues;
                if (!string.IsNullOrWhiteSpace(ListType))
                {
                    switch (ListType)
                    {
                        case "Failed":
                            InitQuery = InitQuery.Where(x => x.Status == "Failed");
                            break;
                        case "Success":
                            InitQuery = InitQuery.Where(x => x.Status == "Success");
                            break;
                        case "InQueue":
                            InitQuery = InitQuery.Where(x => x.Status == "New");
                            break;
                        default:
                            return Error("", "List Type is not supported");
                    }
                }

                QueueList = await InitQuery.OrderByDescending(x => x.CreatedDtm).AsNoTracking().ToListAsync();

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
                EmailContent template = await _dbContext.AppEmailTemplates.Where(x => x.Code == Code).Select(x => new EmailContent { subject = x.Subject, body = x.Content }).AsNoTracking().FirstOrDefaultAsync();

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
                var userName = User?.Identity?.Name ?? "System";
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
                                List<AppEmailQueue> QueueLists = await _dbcontext.AppEmailQueues.Where(x => x.Status != "Successful" && x.CntRetry <= 5).OrderBy(x => x.CreatedDtm).ToListAsync();

                                if (QueueLists.Count > 0)
                                {
                                    foreach (var queue in QueueLists)
                                    {
                                        EmailSenderRs emailRs = await _emailSender.SendEmail(queue.Subject, queue.Content, queue.ToEmail);

                                        queue.CntRetry = queue.CntRetry + 1;
                                        queue.Status = emailRs.Status;
                                        queue.Remark = emailRs.Remars;
                                        queue.ModifiedDtm = DateTime.Now;
                                        queue.ModifiedBy = userName;

                                        if (emailRs.isSuccess)
                                        {
                                            queue.DateSent = DateTime.Now;
                                        }

                                        _dbcontext.AppEmailQueues.Update(queue);
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
    }
}
