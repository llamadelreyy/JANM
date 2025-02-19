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
using DevExpress.Data.Linq.Helpers;
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
                //Dalam Rondaan
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
                var premisTiadaLesen = await _tenantDBContext.mst_premis.Where(p => p.lesen == null || p.lesen == "" || string.IsNullOrEmpty(p.lesen)).CountAsync();
                var premisTamatTempohLesen = await _tenantDBContext.mst_premis.Where(p => p.tempoh_sah_lesen <= DateOnly.FromDateTime(DateTime.Today)).CountAsync();

                var bilCukaiTahunan = await _tenantDBContext.mst_premis.CountAsync();

                ///added by farhana 19/2/2025
                ///pending to get information to sum up :
                //var totalCukaiTaksiranByr 
                //var totalCukaiTaksiranblmByr
                //var totalHslLesenByr
                //var totalHslLesenBlmByr

                var totalKompaunDibyr = await _tenantDBContext.trn_compounds.Where(c => c.trnstatus_id == 5).SumAsync(c => c.amt_cmpd);
                var totalKompaunBlmByr = await _tenantDBContext.trn_compounds.Where(xc => xc.trnstatus_id == 4).SumAsync(c => c.amt_cmpd);

                var result = new dashboard_view
                {
                    total_notis = totalNotis,
                    total_kompaun = totalKompaun,
                    total_inspection = totalNota,
                    total_confiscation = totalSita,
                    premis_berlesen = premisBerlesen,
                    premis_diperiksa = premisDiperiksa,
                    premis_dikenakan_tindakan = totalPremisTindakan,
                    premis_tiada_lesen = premisTiadaLesen,
                    premis_tamat_tempoh_lesen = premisTamatTempohLesen,
                    total_cukai_tahunan = bilCukaiTahunan,
                    amaun_kutipan_cukai = 4569.93M,
                    cukai_taksiran_dibyr = 100230,
                    cukai_taksiran_blm_dibyr= 10230,
                    hsl_lesen_dibyr = 150230,
                    hsl_lesen_blm_dibyr = 15230,
                    kompaun_dibyr = totalKompaunDibyr,
                    kompaun_blm_dibyr = totalKompaunBlmByr,
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


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<dashboard_view>> GetFinancialDashboard()
        {
            try
            {
                var totalLesenAktif = await _tenantDBContext.mst_licensees.Where(l=>l.status_id == 1).CountAsync();
                var totalLesenTamatTempoh = await _tenantDBContext.mst_licensees.Where(l => l.status_id == 2).CountAsync(); 
                var totalPremisPerniagaan = await _tenantDBContext.mst_licensees.GroupBy(x=>x.owner_icno).CountAsync();
                var pertambahanLsnThnSemasa = await _tenantDBContext.mst_licensees.Where(t => t.reg_date.HasValue && t.reg_date.Value.Year == DateTime.Now.Year).CountAsync();
                var pertambahanLsnSemasa = await _tenantDBContext.mst_licensees.Where(t => t.reg_date.HasValue&& t.reg_date.Value.Year == DateTime.Now.Year && t.reg_date.Value.Month == DateTime.Now.Month).CountAsync();

                var result = new dashboard_view
                {
                    total_lesen_aktif = totalLesenAktif,
                    lesen_tamat_tempoh = totalLesenTamatTempoh,
                    total_premis_perniagaan = totalPremisPerniagaan,
                    hsl_tahunan_semasa = 202543.00M,
                    ptmbahan_lesen_thn_semasa = pertambahanLsnThnSemasa,
                    ptmbahan_lesen_semasa = pertambahanLsnSemasa,
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

