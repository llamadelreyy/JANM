/*
Project: PBT Pro
Description: License API controller to handle lesen 
Author: Nurulfarhana
Date: DEcember 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
*/

using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LicenseController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private string LoggerName = "administrator";
        private readonly string _feature = "LESEN";
        public LicenseController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<LicenseController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
        }

        

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await (from license_hist in _dbContext.license_histories
                                join license_info in _dbContext.license_informations
                                on license_hist.hist_id_info equals license_info.license_id
                                group new { license_hist, license_info } by license_hist.hist_id_info into g
                                select new
                                {
                                    Histories = g.Select(x => x.license_hist).ToList(),
                                    LicenseInfo = g.Select(x => x.license_info).ToList(),
                                    HistIdInfo = g.Key
                                })
                                .FirstOrDefaultAsync(x => x.Histories.Any(h => h.license_hist_id == Id));

                if (parFormfield == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                return Ok(parFormfield, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


    }
}
