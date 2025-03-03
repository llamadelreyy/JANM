using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Data;
using System.Reflection;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuditController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuditController> _logger;
        private readonly string _feature = "AUDIT_LOG";

        public AuditController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<AuditController> logger) : base(dbContext)
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
                var parFormfields = await _dbContext.auditlog_infos.AsNoTracking().ToListAsync();

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

        //[AllowAnonymous]
        //[HttpGet("{Id}")]
        //public async Task<ActionResult<auditlog_info>> ViewDetail(int Id)
        //{            
        //    try
        //    {
        //        var parFormfield = await _dbContext.auditlog_infos.FirstOrDefaultAsync(x => x.log_id == Id);

        //        if (parFormfield == null)
        //        {
        //            return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
        //        }
        //        return Ok(parFormfield, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}


        //[AllowAnonymous]
        //[HttpDelete("{Id}")]
        //public async Task<IActionResult> Delete(int Id)
        //{
        //    try
        //    {
        //        string runUser = await getDefRunUser();

        //        #region Validation
        //        var formField = await _dbContext.auditlog_infos.FirstOrDefaultAsync(x => x.log_id == Id);
        //        if (formField == null)
        //        {
        //            return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
        //        }
        //        #endregion

        //        _dbContext.auditlog_infos.Remove(formField);
        //        await _dbContext.SaveChangesAsync();

        //        return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] auditlog_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                auditlog_info auditlog_info = new auditlog_info
                {
                    role_id = InputModel.role_id,
                    module_name = InputModel.module_name,
                    log_descr = InputModel.log_descr,
                    log_type = InputModel.log_type,
                    username = InputModel.username,
                    log_method = InputModel.log_method,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.auditlog_infos.Add(auditlog_info);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    role_id = auditlog_info.role_id,
                    module_name = auditlog_info.module_name,
                    log_descr = auditlog_info.log_descr,
                    log_type = auditlog_info.log_type,
                    username = auditlog_info.username,
                    log_method = auditlog_info.log_method,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                return Ok(result, SystemMesg(_feature, "INSERT_LOG_AUDIT", MessageTypeEnum.Success, string.Format("Berjaya tambah log audit baru.")));
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
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //    finally
        //    {                
        //    }
        //    return Ok("", SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //}
        #endregion

        #region unused
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<auditlog_info>>> ListAuditwithParam(int skip, int? take, string? search)
        //{
        //    if (_dbContext.auditlog_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    var audits = await _dbContext.auditlog_infos
        //       .Where(m =>
        //       m.log_descr.Contains(search) ||
        //       m.username.Contains(search) ||
        //       m.log_method.Contains(search))
        //       .ToListAsync();

        //    int totalRecord = audits.Count();

        //    if (totalRecord == 0) return NoContent("Record not found");

        //    int takeRecord = take.HasValue ? take.Value : 10;
        //    decimal tp = (decimal)totalRecord / (decimal)takeRecord;
        //    int totalpage = (int)Math.Ceiling(tp);

        //    int page = (skip / takeRecord) + 1;

        //    PaginationInfo paginationInfo = new PaginationInfo();
        //    paginationInfo.TotalPages = totalpage;
        //    paginationInfo.TotalRecords = totalRecord;
        //    paginationInfo.RecordPerPage = takeRecord;
        //    paginationInfo.CurrentPageNo = page;

        //    return Ok(audits, paginationInfo, "Sucess");
        //    //return Ok(audits, "Sucess");
        //    //return await _dbContext.TbAuditlogs.ToListAsync();
        //}
        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> InsertAudits(auditlog_info changed, int intCurrentUserId)
        //{
        //    try
        //    {
        //        var runUserID = await getDefRunUserId();
        //        var runUser = await getDefRunUser();

        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("call audit.proc_insertlog(:_audit_role_id, :_audit_module_name, :_audit_description, :_created_by, :_audit_type, :_audit_username, :_audit_method)", myConn))
        //            {
        //                myCmd.CommandType = CommandType.Text;
        //                myCmd.Parameters.AddWithValue("_audit_role_id", DbType.Int32).Value = changed.role_id;
        //                myCmd.Parameters.AddWithValue("_audit_module_name", DbType.String).Value = changed.module_name;
        //                myCmd.Parameters.AddWithValue("_audit_description", DbType.String).Value = changed.log_descr;
        //                myCmd.Parameters.AddWithValue("_created_by", DbType.Int32).Value = changed.creator_id;
        //                myCmd.Parameters.AddWithValue("_audit_type", DbType.Int32).Value = changed.log_type;
        //                myCmd.Parameters.AddWithValue("_audit_username", DbType.String).Value = changed.username;
        //                myCmd.Parameters.AddWithValue("_audit_method", DbType.String).Value = changed.log_method;

        //                myConn.Open();
        //                myCmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught : LogAudit - {0}", ex);
        //        return Ok("", SystemMesg(_feature, "INSERT_LOG_AUDIT", MessageTypeEnum.Success, string.Format("Berjaya tambah log audit baru.")));
        //    }
        //    finally
        //    {
        //    }
        //    return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //}
        #endregion
    }
}
