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
12/2/2025 - api for crud lesen
*/

using AutoMapper.Internal;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using OneOf.Types;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
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
        private readonly IHubContext<PushDataHub> _hubContext;
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
        /*
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

       [HttpGet]
       public async Task<IActionResult> GetList()
       {
           try
           {
               var data = await _tenantDBContext.mst_licensees.Where(e => e.is_deleted == false && e.created_at >= DateTime.Now.AddYears(-5))
                    .Select(e => new mst_licensee
                    {
                        licensee_id = e.licensee_id,
                        license_accno = e.license_accno,
                        owner_icno = e.owner_icno,
                        type_id = e.type_id,
                        business_name = e.business_name,
                        business_addr = e.business_addr,
                        town_code = e.town_code,
                        district_code = e.district_code,
                        state_code = e.state_code,
                        reg_date = e.reg_date ?? DateTime.MinValue,
                        start_date = e.start_date,
                        end_date = e.end_date,
                        status_id = e.status_id,
                        cat_id = e.cat_id ?? 0,
                        ops_id = e.ops_id ?? 0,
                        parl_id = e.parl_id ?? 0,
                        dun_id = e.dun_id ?? 0,
                        zon_id = e.zon_id ?? 0,
                        created_at = e.created_at
                    })
                    .AsNoTracking()
                    .ToListAsync();

               if (data == null)
               {
                   return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
               }

               var result = (
                       from lesen in data
                       join status in _tenantDBContext.ref_license_statuses on lesen.status_id equals status.status_id
                       join ops in _tenantDBContext.ref_license_ops on lesen.ops_id equals ops.ops_id
                       join state in _dbContext.mst_states on lesen.state_code equals state.state_code
                       join district in _dbContext.mst_districts on lesen.district_code equals district.district_code
                       join town in _dbContext.mst_towns on new { lesen.town_code, lesen.district_code }
                       equals new { town.town_code, town.district_code }
                       select new mst_licensee_view
                       {
                           lesen_id = lesen.licensee_id,
                           lesen_acc_no = lesen.license_accno,
                           icno_pemilik = lesen.owner_icno,
                           nama_perniagaan = lesen.business_name,
                           alamat_perniagaan = lesen.business_addr,
                           tarikh_daftar = (DateTime)lesen.reg_date,
                           tarikh_mula_isu = lesen.start_date,
                           tarikh_tamat_isu = lesen.end_date,
                           status_lesen = status.status_name,
                           ops_id = ops.ops_id,
                           ops_name = ops.ops_name,
                           statename = state.state_name,
                           districtname = district.district_name,
                           townname = town.town_name,

                       }).ToList();

               return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
           }
           catch (Exception ex)
           {
               _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
               return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
           }
       }
       */

    }
}
