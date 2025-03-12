/*
Project: PBT Pro
Description: ContactUs API controller to handle Contact Us Form Field
Author: Nurulfarhana
Date: January 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
10/01/2024 - initial create
*/

using DevExpress.XtraPrinting.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Diagnostics;
using System.Reactive.Subjects;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ReportController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportController> _logger;
        private readonly IHubContext<PushDataHub> _hubContext;

        private readonly string _feature = "REPORT";

        public ReportController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<ReportController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
            _logger = logger;
            _hubContext = hubContext;
        }

        /*
            [AllowAnonymous]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<report_view>>> ReportLesen(int opsid = 0, int catid = 0, DateTime? startDate = null, DateTime? endDate = null)
            {
                try
                {
                    startDate ??= DateTime.Today;
                    endDate ??= DateTime.Today.AddMonths(1);

                    var data = await _tenantDBContext.mst_licensees.Where(e => e.is_deleted == false)
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
                                    created_at = e.created_at,
                                    town_id = e.town_id,
                                    ssm_no = e.ssm_no,
                                    gid = e.gid ?? 0,
                                })
                                .AsNoTracking()
                                .ToListAsync();

                    var result = (from license in data
                                  join category in _tenantDBContext.ref_license_cats on license.cat_id equals category.cat_id into kategoriGroup
                                  from category in kategoriGroup.DefaultIfEmpty()
                                  join ops in _tenantDBContext.ref_license_ops on license.ops_id equals ops.ops_id into opsGroup
                                  from ops in opsGroup.DefaultIfEmpty()
                                  where (license.ops_id == opsid || opsid == 0)
                                         && (license.cat_id == catid || catid == 0)
                                         && (startDate == null || license.created_at >= startDate)
                                         && (endDate == null || license.created_at <= endDate)

                                  select new report_view
                                  {
                                      report_id = license.licensee_id,
                                      nama_premis = license.business_name,
                                      alamat = license.business_addr,
                                      no_lesen = license.license_accno,
                                      no_ssm = license.ssm_no,
                                      operasi_id = ops != null ? ops.ops_id : 0,
                                      operasi_lesen = ops != null ? ops.ops_name : null,
                                      kategori_id = category != null ? category.cat_id : 0,
                                      kategori_lesen = category != null ? category.cat_name : null,
                                      created_at = (DateTime)license.created_at,
                                  }
                                ).ToList();

                    return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                    return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                }
            }

            [AllowAnonymous]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<report_view>>> ReportPremisByParlimen(int parlid = 0, DateTime? startDate = null, DateTime? endDate = null)
            {
                try
                {
                    startDate ??= DateTime.Today;
                    endDate ??= DateTime.Today.AddMonths(1);

                    IQueryable<mst_premis> data = (IQueryable<mst_premis>)_dbContext.mst_premis.ToListAsync();

                    var result = (from premis in data                              
                                  //where (premis.ops_id == parlid || parlid == 0)                                    
                                  //       && (startDate == null || premis.created_at >= startDate)
                                  //       && (endDate == null || premis.created_at <= endDate)

                                  select new report_view
                                  {
                                      //report_id = license.licensee_id,
                                      //nama_premis = license.business_name,
                                      //alamat = license.business_addr,
                                      //no_lesen = license.license_accno,
                                      //no_ssm = license.ssm_no,
                                      //operasi_id = ops != null ? ops.ops_id : 0,
                                      //operasi_lesen = ops != null ? ops.ops_name : null,
                                      //kategori_id = category != null ? category.cat_id : 0,
                                      //kategori_lesen = category != null ? category.cat_name : null,
                                      //created_at = (DateTime)license.created_at,
                                  }
                                ).ToList();

                    return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                    return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                }
            }

            [AllowAnonymous]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<report_view>>> ReportPremisByDun(int dunid = 0, DateTime? startDate = null, DateTime? endDate = null)
            {
                try
                {
                    startDate ??= DateTime.Today;
                    endDate ??= DateTime.Today.AddMonths(1);

                    IQueryable<mst_premis> data = (IQueryable<mst_premis>)_dbContext.mst_premis.ToListAsync();

                    var result = (from premis in data
                                      //where (premis.ops_id == parlid || parlid == 0)                                    
                                      //       && (startDate == null || premis.created_at >= startDate)
                                      //       && (endDate == null || premis.created_at <= endDate)

                                  select new report_view
                                  {
                                      //report_id = license.licensee_id,
                                      //nama_premis = license.business_name,
                                      //alamat = license.business_addr,
                                      //no_lesen = license.license_accno,
                                      //no_ssm = license.ssm_no,
                                      //operasi_id = ops != null ? ops.ops_id : 0,
                                      //operasi_lesen = ops != null ? ops.ops_name : null,
                                      //kategori_id = category != null ? category.cat_id : 0,
                                      //kategori_lesen = category != null ? category.cat_name : null,
                                      //created_at = (DateTime)license.created_at,
                                  }
                                ).ToList();

                    return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                    return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                }
            }

            [AllowAnonymous]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<report_view>>> ReportPremisByZon(int zonid = 0, DateTime? startDate = null, DateTime? endDate = null)
            {
                try
                {
                    startDate ??= DateTime.Today;
                    endDate ??= DateTime.Today.AddMonths(1);

                    IQueryable<mst_premis> data = (IQueryable<mst_premis>)_dbContext.mst_premis.ToListAsync();

                    var result = (from premis in data
                                      //where (premis.ops_id == parlid || parlid == 0)                                    
                                      //       && (startDate == null || premis.created_at >= startDate)
                                      //       && (endDate == null || premis.created_at <= endDate)

                                  select new report_view
                                  {
                                      //report_id = license.licensee_id,
                                      //nama_premis = license.business_name,
                                      //alamat = license.business_addr,
                                      //no_lesen = license.license_accno,
                                      //no_ssm = license.ssm_no,
                                      //operasi_id = ops != null ? ops.ops_id : 0,
                                      //operasi_lesen = ops != null ? ops.ops_name : null,
                                      //kategori_id = category != null ? category.cat_id : 0,
                                      //kategori_lesen = category != null ? category.cat_name : null,
                                      //created_at = (DateTime)license.created_at,
                                  }
                                ).ToList();

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
