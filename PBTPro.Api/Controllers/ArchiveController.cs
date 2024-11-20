using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;

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
        private List<auditlog_archive_info> _Faq { get; set; }
        public ArchiveController(PBTProDbContext dbContext, ILogger<ArchiveController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");//configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

       
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<auditlog_archive_info>>> ListAudit()
        {
            if (_dbContext.auditlog_archive_infos == null)
            {
                return NotFound();
            }
            return await _dbContext.auditlog_archive_infos.ToListAsync();           
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<auditlog_archive_info>> RetrieveAudit(int id)
        {
            if (_dbContext.auditlog_archive_infos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.auditlog_archive_infos.FindAsync(id);

            if (audit == null)
            {
                return NotFound();
            }

            return audit;
        }
      

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            if (_dbContext.auditlog_archive_infos == null)
            {
                return NotFound();
            }
            var audit = await _dbContext.auditlog_archive_infos.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }

            _dbContext.auditlog_archive_infos.Remove(audit);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditExists(int id)
        {
            return (_dbContext.auditlog_archive_infos?.Any(e => e.archive_id == id)).GetValueOrDefault();
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
