using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArchiveController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ArchiveController> _logger;
        private readonly string _feature = "ARCHIVE_AUDIT_LOG";

        public ArchiveController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<ArchiveController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListAll()
        {
            try
            {
                var parFormfields = await _dbContext.auditlog_archive_infos.AsNoTracking().ToListAsync();

                if (parFormfields.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(parFormfields, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Archived auditlog
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> Archive()
        //{
        //    try
        //    {
        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT audit.func_copy_archive_audit_logs()", myConn))
        //            {                       
        //                myConn.Open();
        //                myCmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //    finally
        //    {
        //    }
        //    return Ok("", SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //}


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ArchiveByMonth(int dtm)
        {
            try
            {
                var strMonth = DateTime.Now.AddMonths(-dtm);

                #region retrieve auditlog and copy to archive table
                var dataFromTableA = _dbContext.auditlog_infos.AsNoTracking().ToList();

                var dataForTableB = dataFromTableA.Where(x => x.created_at <= strMonth).Select(a => new auditlog_archive_info
                {
                    archive_id = a.log_id,
                    role_id = a.role_id,
                    archive_module_name = a.module_name,
                    archive_desc = a.log_descr,
                    creator_id = a.creator_id,
                    created_at = a.created_at,
                    archive_type = a.log_type,
                    archive_username = a.username,
                    archive_method = a.log_method,
                    is_archived = true
                })
                .Where(x => !_dbContext.auditlog_archive_infos.Any(a => a.archive_id == x.archive_id))
                .ToList();

                if (!dataForTableB.Any())
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Tiada rekod untuk diarkibkan dalam tempoh {0} bulan lepas.", dtm)));
                }

                _dbContext.auditlog_archive_infos.AddRange(dataForTableB);
                _dbContext.SaveChanges();
                #endregion

                #region record to delete from table audit log after archive
                var recordsToDelete = _dbContext.auditlog_infos.Where(x => x.created_at <= strMonth).ToList();

                _dbContext.auditlog_infos.RemoveRange(recordsToDelete);
                _dbContext.SaveChanges();
                #endregion

                await Task.Delay(1000);

                #region display list archive log
                var dataFromTableB_b = _dbContext.auditlog_infos.AsNoTracking().ToList();

                if (dataFromTableB_b.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }
                return Ok(dataFromTableB_b, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

    }
}
