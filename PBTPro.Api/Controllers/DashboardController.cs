/*
Project: PBT Pro
Description: Dashboard ZonEksekutif API controller to handle reporting
Author: Nurulfarhana
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
Ticket No: 053
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Data;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DashboardController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        protected PBTProTenantDbContext _tenantDBContext;
        private readonly ILogger<DashboardController> _logger;
        private readonly string _feature = "DASHBOARD";

        public DashboardController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<DashboardController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }


        #region jenis-jenis saman

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<dashboard_view>> GetDashboardData()
        {
            try
            {
                var totalNotis = await _tenantDBContext.trn_notices.CountAsync();
                var totalKompaun = await _tenantDBContext.trn_compounds.CountAsync();
                var totalNota = await _tenantDBContext.trn_inspections.CountAsync();
                var totalSita = await _tenantDBContext.trn_confiscations.CountAsync();
                var premisBerlesen = await _tenantDBContext.mst_licensees.Where(t => t.status_id == 1).CountAsync();
                var premisDiperiksa = await _tenantDBContext.mst_patrol_schedules.Where(t => t.status_id == 2).CountAsync();

                var premisTindakan = await _tenantDBContext.mst_licensees
                    .Select(lesen => new
                    {
                        lesen.license_accno,
                        NoticeCount = _tenantDBContext.trn_notices.Count(notis => notis.license_accno == lesen.license_accno),
                        CompoundCount = _tenantDBContext.trn_compounds.Count(kompaun => kompaun.license_accno == lesen.license_accno),
                        ConfiscationCount = _tenantDBContext.trn_confiscations.Count(sita => sita.license_accno == lesen.license_accno)
                    })
                    .ToListAsync();

                var totalPremisTindakan = premisTindakan.Sum(x => x.NoticeCount + x.CompoundCount + x.ConfiscationCount);

                var result = new dashboard_view
                {
                    total_notis = totalNotis,
                    total_kompaun = totalKompaun,
                    total_inspection = totalNota,
                    total_confiscation = totalSita,
                    premis_berlesen = premisBerlesen,
                    premis_diperiksa = premisDiperiksa,
                    premis_dikenakan_tindakan = totalPremisTindakan, 
                };

                if (result == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }


        }

        #endregion


        #region stoc proc example       

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> JumlahNotisSP()
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT tenant.func_totalnotices()", myConn))
                    {
                        myConn.Open();

                        var total = myCmd.ExecuteScalar();

                        if (total == null)
                        {
                            return Error("", SystemMesg("COMMON", "NO_DATA", MessageTypeEnum.Error, "No data found."));
                        }
                        int totalCount = Convert.ToInt32(total);

                        return Ok(new { totalCount = totalCount }, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, "Senarai rekod berjaya dijana"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
            finally
            {
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> JumlahPremisBerlesenSP()
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT tenant.func_totalpremislicense()", myConn))
                    {
                        myConn.Open();

                        var total = myCmd.ExecuteScalar();

                        if (total == null)
                        {
                            return Error("", SystemMesg("COMMON", "NO_DATA", MessageTypeEnum.Error, "No data found."));
                        }
                        int totalCount = Convert.ToInt32(total);

                        return Ok(new { totalCount = totalCount }, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, "Senarai rekod berjaya dijana"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
            finally
            {
            }
        }
        #endregion

    }
}

