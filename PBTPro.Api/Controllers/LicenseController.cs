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

using AutoMapper.Internal;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using static Duende.IdentityServer.Models.IdentityResources;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LicenseController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LicenseController> _logger;

        private readonly string _feature = "LESEN";
        public LicenseController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<LicenseController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
            _tenantDBContext = tntdbContext;
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

        [HttpGet]
        public async Task<IActionResult> GetListByTabType(string tabType)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                var result = from lesen in _tenantDBContext.mst_licensees
                             where lesen.created_at >= DateTime.Now.AddYears(-5)
                             orderby lesen.licensee_id ascending
                             select new premis_history_view
                             {
                                 gid = lesen.licensee_id,
                                 no_acc_lesen = lesen.license_accno,
                                 no_ic_pemilik = lesen.owner_icno,
                                 nama_pemilik = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.mst_owners,
                                                                      lesen1 => lesen1.owner_icno,
                                                                      pemilik => pemilik.owner_icno,
                                                                      (lesen1, pemilik) => new { lesen1, pemilik })
                                                                .Where(x => x.lesen1.owner_icno == lesen.owner_icno)
                                                                .Select(x => x.pemilik.owner_name)
                                                                .FirstOrDefault(),
                                 nama_perniagaan = lesen.business_name,
                                 alamat_premis = lesen.business_addr,
                                 tarikh_daftar = lesen.reg_date,
                                 nama_ops = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.ref_license_ops,
                                                                      ops => ops.ops_id,
                                                                      opsType => opsType.ops_id,
                                                                      (ops, opsType) => new { ops, opsType })
                                                                .Where(x => x.ops.license_accno == lesen.license_accno)
                                                                .Select(x => x.opsType.ops_name)
                                                                .FirstOrDefault(),
                                 nama_parlimen = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.mst_parliaments,
                                                                      lesen1 => lesen1.parl_id,
                                                                      parliment => parliment.parl_id,
                                                                      (lesen1, parliment) => new { lesen1, parliment })
                                                                .Where(x => x.lesen1.license_accno == lesen.license_accno)
                                                                .Select(x => x.parliment.parl_name)
                                                                .FirstOrDefault(),
                                 nama_dun = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.mst_duns,
                                                                      lesen1 => lesen1.dun_id,
                                                                      dun => dun.dun_id,
                                                                      (lesen1, dun) => new { lesen1, dun })
                                                                .Where(x => x.lesen1.license_accno == lesen.license_accno)
                                                                .Select(x => x.dun.dun_name)
                                                                .FirstOrDefault(),
                                 nama_zon = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.mst_zons,
                                                                      lesen1 => lesen1.zon_id,
                                                                      zon => zon.zon_id,
                                                                      (lesen1, zon) => new { lesen1, zon })
                                                                .Where(x => x.lesen1.license_accno == lesen.license_accno)
                                                                .Select(x => x.zon.zon_code)
                                                                .FirstOrDefault(),
                                 status_lesen_perniagaan = _tenantDBContext.mst_licensees
                                                                .Join(_tenantDBContext.ref_license_statuses,
                                                                      lesen1 => lesen1.status_id,
                                                                      status => status.status_id,
                                                                      (lesen1, status) => new { lesen1, status })
                                                                .Where(x => x.lesen1.license_accno == lesen.license_accno)
                                                                .Select(x => x.status.status_name)
                                                                .FirstOrDefault(),
                                 CreatedAt = (DateTime)lesen.created_at,
                             };

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
