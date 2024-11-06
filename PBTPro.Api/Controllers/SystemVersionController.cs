using Microsoft.AspNetCore.Mvc;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.Shared.Models;
using PBTPro.Shared.Models.SystemVersion;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemVersionController : IBaseController
    {
        private readonly ILogger<SystemVersionController> _logger;

        public SystemVersionController(PBTProDbContext dbContext, ILogger<SystemVersionController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllActionSetting")]
        public IEnumerable<VersionInformation> GetAllVersionSystem()
        {
            var versionInformation = new List<VersionInformation>();

            //versionInformation.Add(new VersionInformation { VersionId = 1, VersionNumber = "Kompaun", VersionName = "Jenis Tindakan", VersionDescription = "Jenis Tindakan" });
            //versionInformation.Add(new VersionInformation { VersionId = 2, VersionNumber = "Notis", VersionName = "Jenis Tindakan", VersionDescription = "Jenis Tindakan" });

            return versionInformation;
        }
    }
}
