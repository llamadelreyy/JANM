using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MessageBlasterController : IBaseController
    {
        private readonly ILogger<MessageBlasterController> _logger;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "MSGBLASTER";

        public MessageBlasterController(PBTProDbContext dbContext, ILogger<MessageBlasterController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public class MessageRequest
        {
            public string User { get; set; }
            public string Message { get; set; }
        }

        public class UserMessageRequest
        {
            public string User { get; set; }
            public List<string> UserId { get; set; }
            public string Message { get; set; }
        }

        public class GroupMessageRequest
        {
            public string GroupName { get; set; }
            public string User { get; set; }
            public string Message { get; set; }
        }

        [HttpPost]
        [Route("sendMessage")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);                   
                return Ok("", SystemMesg(_feature, "SEND_MSG", MessageTypeEnum.Success, string.Format("Message berjaya dihantar")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }

        }

        [HttpPost]
        [Route("sendToUser")]
        [AllowAnonymous]
        public async Task<IActionResult> sendToUser([FromBody] UserMessageRequest request)
        {
            try
            {
                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);
                foreach (var userId in request.UserId)
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == userId).Select(kvp => kvp.Key).FirstOrDefault();
                    //var userConnection = connectedUsers.FirstOrDefault(x => x.Key == userId);
                    //string? connectionId = userConnection.Value;
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", request.User, request.Message);
                    }
                }
                return Ok("", SystemMesg(_feature, "SEND_MSG", MessageTypeEnum.Success, string.Format("Message berjaya dihantar")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }

        }

        [HttpPost]
        [Route("sendToGroup")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageToGroup([FromBody] GroupMessageRequest request)
        {
            try
            {
                await _hubContext.Clients.Group(request.GroupName).SendAsync("ReceiveMessage", request.User, request.Message);
                return Ok("", SystemMesg(_feature, "SEND_MSG", MessageTypeEnum.Success, string.Format("Message berjaya dihantar")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
