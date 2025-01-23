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

        private string LoggerName = "administrator";
        private readonly string _feature = "DASHBOARD";

        public DashboardController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<DashboardController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
        }

        #region stoc proc example       

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> TotalNoticesSP()
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
            finally
            {
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> TotalPremisLicenseSP()
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
            finally
            {
            }
        }
        #endregion

        #region jenis-jenis notis
        /// <summary>
        /// item 1. Notis
        /// </summary>
        /// <returns></returns>       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalNotice()
        {
            try
            {
                var result = await _tenantDBContext.trn_notices.CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item 2. kompaun
        /// </summary>
        /// <returns></returns>       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalCompound()
        {
            try
            {
                var result = await _tenantDBContext.trn_compounds.CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item 3. Nota Pemeriksaan
        /// </summary>
        /// <returns></returns>       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalInspection()
        {
            try
            {
                var result = await _tenantDBContext.trn_inspections.CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        /// <summary>
        /// item 4. Nota Pemeriksaan
        /// </summary>
        /// <returns></returns>       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalConfiscations()
        {
            try
            {
                var result = await _tenantDBContext.trn_confiscations.CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #endregion

        #region Jumlah Premis berlesen

        /// <summary>
        /// item c) jumlah premis berlesen
        /// </summary>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalPremisLicense()
        {
            try
            {
                var result = await _tenantDBContext.mst_premis.Where(x => x.lesen != null).CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));

            }
        }


        #endregion

        #region Jumlah Premis tidak berlesen

        /// <summary>
        /// item c) jumlah premis tidak berlesen
        /// </summary>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TotalPremisNoLicense()
        {
            try
            {
                var result = await _tenantDBContext.mst_premis.Where(x => x.lesen == null).CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));

            }
        }


        #endregion

        #region tiket backlog
        /// <summary>
        /// item a) Jumlah lesen aktif
        /// </summary>
        /// <returns></returns>       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlActiveLicense()
        {
            try
            {
                var result = await _tenantDBContext.mst_licensees.Where(x => x.status_id == 1).CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item b) jumlah lesen tamat tempoh
        /// </summary>
        /// <returns></returns> 
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlExpiredLicense()
        {
            try
            {
                var result = await _tenantDBContext.mst_licensees.Where(x => x.status_id == 2).CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

       

        /// <summary>
        /// item d) hasil tahunan semasa
        /// </summary>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlCurrentYear()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var startOfYear = new DateTime(currentYear, 1, 1);
                var endOfYear = new DateTime(currentYear, 12, 31);
                var result = await _dbContext.license_informations.Where(x => x.created_date >= startOfYear && x.created_date <= endOfYear).SumAsync(x => x.license_amount);
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item e) jumlah lesen berisiko
        /// </summary>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlRiskLicense()
        {
            try
            {
                var result = await _dbContext.license_informations.Where(x => x.license_risk_status == "Risiko").CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item f) potensi hasil belum dikutip
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlPotentialRslt()
        {
            try
            {
                var result = await (from li in _dbContext.license_informations
                                    join lt in _dbContext.license_transactions on li.license_id equals lt.license_trans_info
                                    where li.license_payment_status == "Pending"
                                    group li by li.license_id into g
                                    select g.Sum(li => li.license_amount)).FirstOrDefaultAsync();
                if (result == null)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item g) jumlah lesen tidak berisiko
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlNotRiskLicense()
        {
            try
            {
                int result = await _dbContext.license_informations
                            .Where(x => x.license_risk_status == "Tidak Berisiko").CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item h) pertambahan lesen tahun semasa
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlAddedLicenseCurrYr()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var startOfYear = new DateTime(currentYear, 1, 1);
                var endOfYear = new DateTime(currentYear, 12, 31);

                var result = await _dbContext.license_informations
                            .Where(x => x.created_date >= startOfYear && x.created_date <= endOfYear)
                            .CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item i) potensi perniagaan tanpa lesen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlPotentialRsltWoLicense()
        {
            try
            {
                var result = await _dbContext.license_informations
                            .Where(x => x.license_risk_status == "Tanpa Lesen").CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        /// <summary>
        /// item j) pertambahan lesen bulan semasa
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlAddedLicenseCurrMth()
        {
            try
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var result = await _dbContext.license_informations
                            .Where(x => x.created_date >= startOfMonth && x.created_date <= endOfMonth)
                            .CountAsync();
                if (result == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
    }
}

