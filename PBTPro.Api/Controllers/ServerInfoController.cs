/*
Project: PBT Pro
Description: simple api to be used as mobile apps connection checker
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
21/11/2024 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ServerInfoController : IBaseController
    {
        private readonly ILogger<SystemVersionController> _logger;

        public ServerInfoController(PBTProDbContext dbContext, ILogger<SystemVersionController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetHeartBeat()
        {            
            return Ok(new {HeartBeat = "Ok", TimeStamp = DateTime.Now});
        }
    }
}
