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
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DashboardController : IBaseController
    {       
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;

        private string LoggerName = "administrator";
        private readonly string _feature = "DASHBOARD";

        public DashboardController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<DashboardController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
        }

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
                var result = await _dbContext.license_informations.Where(x => x.license_status == "Aktif").CountAsync();
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
                var result = await _dbContext.license_informations.Where(x => x.license_status == "Tamat Tempoh").CountAsync();
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
        /// item c) jumlah premis perniagaan
        /// </summary>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<int>> TtlBusinnessPremise()
        {
            try
            {
                var result = await _dbContext.license_informations.Where(x => x.license_type == "Bisness").CountAsync();
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
    }
}

