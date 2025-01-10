using Microsoft.AspNetCore.Mvc;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Services;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundTaskController : IBaseController
    {
        private readonly ILogger<BackgroundTaskController> _logger;
        private readonly PBTProBkgdSM _bkgdSM;
        private readonly IEmailSender _emailSender;
        private readonly string _feature = "BKGRTSK";
        private readonly int _maxRetry = 5;

        public BackgroundTaskController(PBTProDbContext dbContext, ILogger<BackgroundTaskController> logger, IEmailSender emailSender, PBTProBkgdSM bkgdSM) : base(dbContext)
        {
            _logger = logger;
            _emailSender = emailSender;
            _bkgdSM = bkgdSM;
        }

        [HttpGet("getAllBkgrService")]
        public IActionResult GetAllBkgrService()
        {
            var result = _bkgdSM.GetAllBackgroundServiceStatus();
            return Ok(result);
        }

        [HttpGet("getQueueBkgrService/{serviceName}")]
        public IActionResult GetCntQueueBkgrService(string serviceName)
        {
            var result = _bkgdSM.GetBackgroundServiceQueue(serviceName);
            return Ok(result?.PendingWork?.Count);
        }

        [HttpPost("startBkgrService/{serviceName}")]
        public IActionResult StartBackgroundService(string serviceName)
        {
            try
            {
                _bkgdSM.StartBackgroundService(serviceName);
                _bkgdSM.EnqueueWorkItem(serviceName, async token =>
                {
                    try
                    {
                        int maxLine = 2000000000;
                        int i = 0;
                        using (StreamWriter writer = new StreamWriter($"/data/IWKTest/{serviceName}.txt"))
                        {
                            while (i < maxLine)
                            {
                                i++;
                                writer.WriteLine(i);
                                token.ThrowIfCancellationRequested();
                            }
                        }
                    }
                    catch (OperationCanceledException OCex)
                    {
                        _logger.LogError(string.Format($"{0} Message : {1}, Inner Exception {2}", _feature, "Stop By User", OCex.InnerException));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }

            return Ok($"Background service '{serviceName}' started.");
        }


        [HttpPost("stopBkgrService/{serviceName}")]
        public IActionResult StopBackgroundService(string serviceName)
        {
            _bkgdSM.RemoveBackgroundService(serviceName);
            return Ok($"Background service '{serviceName}' stopped.");
        }
    }
}
