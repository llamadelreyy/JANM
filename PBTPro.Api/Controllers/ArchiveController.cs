using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArchiveController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<ArchiveController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PBTProDbContext _dbContext;
        protected readonly CommonFunction _cf;
        
        private string LoggerName = "";
        private readonly string _feature = "ARCHIVE_AUDIT_LOG";

        private List<auditlog_archive_info> _Archive { get; set; }
        public ArchiveController(PBTProDbContext dbContext, ILogger<ArchiveController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");
            _dbContext = dbContext;
            _cf = new CommonFunction(httpContextAccessor, configuration);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListAudit()
        {
            try
            {
                var parFormfields = await _dbContext.auditlog_archive_infos.AsNoTracking().ToListAsync();

                if (parFormfields.Count == 0)
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tiada rekod untuk dipaparkan", 1, LoggerName, "");
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Pangkalan API untuk paparan senarai arkib log audit. ", 1, LoggerName, "");
                return Ok(parFormfields, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Archived auditlog
        [AllowAnonymous]
        //[HttpPost("InsertArchiveAudit")]
        [HttpGet]
        public async Task<IActionResult> InsertArchiveAudit()
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT audit.func_copy_archive_audit_logs()", myConn))
                    {                       
                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
            finally
            {
            }
            await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Pangkalan API untuk arkib log audit.", 1, LoggerName, "");
            return Ok("", SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        }
        #endregion

    }
}
