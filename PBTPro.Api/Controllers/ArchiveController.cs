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
    public class ArchiveController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<ArchiveController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Archive";
        private readonly PBTProDbContext _dbContext;
        private List<AuditlogArchiveInfo> _Faq { get; set; }
        public ArchiveController(PBTProDbContext dbContext, ILogger<ArchiveController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");//configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditlogArchiveInfo>>> ListAudit()
        {
            if (_dbContext.AuditlogArchiveInfos == null)
            {
                return NotFound();
            }
            return await _dbContext.AuditlogArchiveInfos.ToListAsync();           
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditlogArchiveInfo>> RetrieveAudit(int id)
        {
            if (_dbContext.AuditlogArchiveInfos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.AuditlogArchiveInfos.FindAsync(id);

            if (audit == null)
            {
                return NotFound();
            }

            return audit;
        }
      

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            if (_dbContext.AuditlogArchiveInfos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.AuditlogArchiveInfos.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }

            _dbContext.AuditlogArchiveInfos.Remove(audit);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditExists(int id)
        {
            return (_dbContext.AuditlogArchiveInfos?.Any(e => e.ArchiveId == id)).GetValueOrDefault();
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
