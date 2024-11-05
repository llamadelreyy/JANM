using Microsoft.AspNetCore.Mvc;
using PBTPro.Shared.Models;

namespace PBTPro.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigSettingController : ControllerBase
    {
        private readonly ILogger<ConfigSettingController> _logger;

        public ConfigSettingController(ILogger<ConfigSettingController> logger)
        {
            _logger = logger;
        }
        [HttpGet(Name = "GetAllActionSetting")]
        public IEnumerable<Shared.Models.Action> GetAllActionSetting()
        {
            var actionSettings = new List<Shared.Models.Action>();

            actionSettings.Add(new Shared.Models.Action { Actionid = 1, Actionname = "Kompaun", Actiondescription = "Jenis Tindakan", Actionenabled = true });
            actionSettings.Add(new Shared.Models.Action { Actionid = 2, Actionname = "Notis", Actiondescription = "Jenis Tindakan", Actionenabled = true });

            return actionSettings;
        }
    }
}