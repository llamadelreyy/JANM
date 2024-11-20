using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        private readonly string _module = "Audit";
        private readonly PBTProDbContext _dbContext;
        private List<auditlog_info> _Faq { get; set; }
        public AuditController(PBTProDbContext dbContext, ILogger<AuditController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");//configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<auditlog_info>>> ListAudit()
        {
            if (_dbContext.auditlog_infos == null)
            {
                return NotFound();
            }
            return await _dbContext.auditlog_infos.ToListAsync();           
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<auditlog_info>> RetrieveAudit(int id)
        {
            if (_dbContext.auditlog_infos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.auditlog_infos.FindAsync(id);

            if (audit == null)
            {
                return NotFound();
            }

            return audit;
        }
      

        [AllowAnonymous]
        [HttpPost]
        public Task<bool> InsertAudit(auditlog_info changed, int intCurrentUserId)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            if (_dbContext.auditlog_infos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.auditlog_infos.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }

            _dbContext.auditlog_infos.Remove(audit);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditExists(int id)
        {
            return (_dbContext.auditlog_infos?.Any(e => e.audit_id == id)).GetValueOrDefault();
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
                Console.WriteLine("Exception caught : InsertArchiveAudit - {0}", ex);
                return null;
            }
            finally
            {                
            }
            return Ok();
        }
        #endregion

    }
}
