//using Microsoft.AspNetCore.Mvc;
//using PBTPro.Shared.Models.SystemVersion;
//using PBTPro.Shared.Models.TetapanTindakan;

//namespace PBTPro.Api.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class SystemVersionController : ControllerBase
//    {
//        private readonly ILogger<SystemVersionController> _logger;

//        public SystemVersionController(ILogger<SystemVersionController> logger)
//        {
//            _logger = logger;
//        }
//        [HttpGet(Name = "GetAllActionSetting")]
//        public IEnumerable<VersionInformation> GetAllVersionSystem()
//        {
//            var versionInformation = new List<VersionInformation>();

//            //versionInformation.Add(new VersionInformation { VersionId = 1, VersionNumber = "Kompaun", VersionName = "Jenis Tindakan", VersionDescription = "Jenis Tindakan" });
//            //versionInformation.Add(new VersionInformation { VersionId = 2, VersionNumber = "Notis", VersionName = "Jenis Tindakan", VersionDescription = "Jenis Tindakan" });

//            return versionInformation;
//        }
//    }
//}
