using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.Shared.Models.TetapanTindakan;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigSettingController : IBaseController
    {
        private readonly ILogger<ConfigSettingController> _logger;

        public ConfigSettingController(PBTProDbContext dbContext, ILogger<ConfigSettingController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllActionSetting")]
        public async Task<IActionResult> GetAllActionSetting()
        {
            var actionSettings = new List<ActionSetting>();

            actionSettings.Add(new ActionSetting { ActionId = 1, ActionName="Kompaun", ActionDescription="Jenis Tindakan", ActionEnabled=true});
            actionSettings.Add(new ActionSetting { ActionId = 2, ActionName = "Notis", ActionDescription = "Jenis Tindakan", ActionEnabled = true });

            return Ok(actionSettings, "Result Found");
        }
    }
}
