using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OneOf.Types;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Data;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuditController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<AuditController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PBTProDbContext _dbContext; 
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "AUDIT_LOG";
        private List<auditlog_info> _Faq { get; set; }
        public AuditController(PBTProDbContext dbContext, ILogger<AuditController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListAudit()
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }       

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<ActionResult<auditlog_info>> RetrieveAudit(int Id)
        {            
            try
            {
                var parFormfield = await _dbContext.auditlog_infos.FirstOrDefaultAsync(x => x.audit_id == Id);

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


        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAudit(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.auditlog_infos.FirstOrDefaultAsync(x => x.audit_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.auditlog_infos.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InsertAudit([FromBody] auditlog_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);

                #region store data
                auditlog_info auditlog_info = new auditlog_info
                {
                    audit_role_id = InputModel.audit_role_id,
                    audit_module_name = InputModel.audit_module_name,
                    audit_description = InputModel.audit_description,
                    audit_type = InputModel.audit_type,
                    audit_username = InputModel.audit_username,
                    audit_method = InputModel.audit_method,
                    created_by = runUserID,
                    created_date = DateTime.Now,
                };

                _dbContext.auditlog_infos.Add(auditlog_info);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    audit_role_id = auditlog_info.audit_role_id,
                    audit_module_name = auditlog_info.audit_module_name,
                    audit_description = auditlog_info.audit_description,
                    audit_type = auditlog_info.audit_type,
                    audit_username = auditlog_info.audit_username,
                    audit_method = auditlog_info.audit_method,
                    created_by = auditlog_info.created_by,
                    created_date = auditlog_info.created_date
                };

                return Ok(result, SystemMesg(_feature, "INSERT_LOG_AUDIT", MessageTypeEnum.Success, string.Format("Berjaya tambah log audit baru.")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool AuditExists(int id)
        {
            return (_dbContext.auditlog_infos?.Any(e => e.audit_id == id)).GetValueOrDefault();
        }

        [AllowAnonymous]
        [HttpPost]
        public Task<bool> InsertAudits(auditlog_info changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call audit.proc_insertaudit(:_audit_role_id, :_audit_module_name, :_audit_description, :_created_by, :_audit_type, :_audit_username, :_audit_method)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_audit_role_id", DbType.Int32).Value = changed.audit_role_id;
                        myCmd.Parameters.AddWithValue("_audit_module_name", DbType.String).Value = changed.audit_module_name;                        
                        myCmd.Parameters.AddWithValue("_audit_description", DbType.String).Value = changed.audit_description;
                        myCmd.Parameters.AddWithValue("_created_by", DbType.Int32).Value = changed.created_by;
                        myCmd.Parameters.AddWithValue("_audit_type", DbType.Int32).Value = changed.audit_type;
                        myCmd.Parameters.AddWithValue("_audit_username", DbType.String).Value = changed.audit_username;
                        myCmd.Parameters.AddWithValue("_audit_method", DbType.String).Value = changed.audit_method;

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : InsertAudit - {0}", ex);
                return Task.FromResult(false); ;
            }
            finally
            {
            }
            return Task.FromResult(true);
        }


        #region Archived auditlog
        [AllowAnonymous]
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
                Console.WriteLine("Exception caught : InsertArchiveAudit - {0}", ex);
                return null;
            }
            finally
            {                
            }
            return Ok("", SystemMesg(_feature, "ARCHIVE_LOG_AUDIT", MessageTypeEnum.Success, string.Format("Berjaya arkib log audit baru.")));
        }
        #endregion

        #region unused
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<auditlog_info>>> ListAuditwithParam(int skip, int? take, string? search)
        {
            if (_dbContext.auditlog_infos == null)
            {
                return NotFound();
            }
            var audits = await _dbContext.auditlog_infos
               .Where(m =>
               m.audit_description.Contains(search) ||
               m.audit_username.Contains(search) ||
               m.audit_method.Contains(search))
               .ToListAsync();

            int totalRecord = audits.Count();

            if (totalRecord == 0) return NoContent("Record not found");

            int takeRecord = take.HasValue ? take.Value : 10;
            decimal tp = (decimal)totalRecord / (decimal)takeRecord;
            int totalpage = (int)Math.Ceiling(tp);

            int page = (skip / takeRecord) + 1;

            PaginationInfo paginationInfo = new PaginationInfo();
            paginationInfo.TotalPages = totalpage;
            paginationInfo.TotalRecords = totalRecord;
            paginationInfo.RecordPerPage = takeRecord;
            paginationInfo.CurrentPageNo = page;

            return Ok(audits, paginationInfo, "Sucess");
            //return Ok(audits, "Sucess");
            //return await _dbContext.TbAuditlogs.ToListAsync();
        }

        #endregion
    }
}
