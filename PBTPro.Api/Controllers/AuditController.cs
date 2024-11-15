using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.Common;
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
        private List<AuditlogInfo> _Faq { get; set; }
        public AuditController(PBTProDbContext dbContext, ILogger<AuditController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");//configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditlogInfo>>> ListAuditwithParam(int skip, int? take, string? search)
        {
            if (_dbContext.AuditlogInfos == null)
            {
                return NotFound();
            }
            var audits = await _dbContext.AuditlogInfos
               .Where(m =>
               m.AuditDescription.Contains(search) ||
               m.AuditUsername.Contains(search) ||
               m.AuditMethod.Contains(search))
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
            //return await _dbContext.TbAuditlogs.ToListAsync();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditlogInfo>>> ListAudit()
        {
            if (_dbContext.AuditlogInfos == null)
            {
                return NotFound();
            }
            return await _dbContext.AuditlogInfos.ToListAsync();           
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditlogInfo>> RetrieveAudit(int id)
        {
            if (_dbContext.AuditlogInfos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.AuditlogInfos.FindAsync(id);

            if (audit == null)
            {
                return NotFound();
            }

            return audit;
        }
      

        [AllowAnonymous]
        [HttpPost]
        public Task<bool> InsertAudit(AuditlogInfo changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call audit.proc_insertaudit(:_audit_role_id, :_audit_module_name, :_audit_description, :_created_by, :_audit_type, :_audit_username, :_audit_method)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_audit_role_id", DbType.Int32).Value = changed.AuditRoleId;
                        myCmd.Parameters.AddWithValue("_audit_module_name", DbType.String).Value = changed.AuditModuleName;                        
                        myCmd.Parameters.AddWithValue("_audit_description", DbType.String).Value = changed.AuditDescription;
                        myCmd.Parameters.AddWithValue("_created_by", DbType.Int32).Value = changed.CreatedBy;
                        myCmd.Parameters.AddWithValue("_audit_type", DbType.Int32).Value = changed.AuditType;
                        myCmd.Parameters.AddWithValue("_audit_username", DbType.String).Value = changed.AuditUsername;
                        myCmd.Parameters.AddWithValue("_audit_method", DbType.String).Value = changed.AuditMethod;

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
            if (_dbContext.AuditlogInfos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.AuditlogInfos.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }

            _dbContext.AuditlogInfos.Remove(audit);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditExists(int id)
        {
            return (_dbContext.AuditlogInfos?.Any(e => e.AuditId == id)).GetValueOrDefault();
        }

        #region Archived auditlog
        [AllowAnonymous]
        [HttpPost]
        public Task<bool> InsertArchiveAudit(AuditlogInfo changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call audit.proc_insertaudit(:_audit_role_id, :_audit_module_name, :_audit_description, :_created_by, :_audit_type, :_audit_username, :_audit_method)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_audit_role_id", DbType.Int32).Value = changed.AuditRoleId;
                        myCmd.Parameters.AddWithValue("_audit_module_name", DbType.String).Value = changed.AuditModuleName;
                        myCmd.Parameters.AddWithValue("_audit_description", DbType.String).Value = changed.AuditDescription;
                        myCmd.Parameters.AddWithValue("_created_by", DbType.Int32).Value = changed.CreatedBy;
                        myCmd.Parameters.AddWithValue("_audit_type", DbType.Int32).Value = changed.AuditType;
                        myCmd.Parameters.AddWithValue("_audit_username", DbType.String).Value = changed.AuditUsername;
                        myCmd.Parameters.AddWithValue("_audit_method", DbType.String).Value = changed.AuditMethod;

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
        #endregion

    }
}
