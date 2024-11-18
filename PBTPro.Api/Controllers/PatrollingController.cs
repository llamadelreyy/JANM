/*
Project: PBT Pro
Description: Patrolling API controller to handle Patrolling Form Field
Author: Nurulfarhana
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PatrollingController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<PatrollingController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Patrolling";
        private readonly PBTProDbContext _dbContext;
        private List<PatrollingInfo> _Patrolling { get; set; }

        public PatrollingController(PBTProDbContext dbContext, ILogger<PatrollingController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatrollingInfo>>> ListPatrolling()
        {
            if (_dbContext.PatrollingInfos == null)
            {
                return NotFound();
            }
            return await _dbContext.PatrollingInfos.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatrollingInfo>> RetrievePatrolling(int id)
        {
            if (_dbContext.PatrollingInfos == null)
            {
                return NotFound();
            }
            var patrolling = await _dbContext.PatrollingInfos.FindAsync(id);

            if (patrolling == null)
            {
                return NotFound();
            }

            return patrolling;
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<ActionResult<PatrollingInfo>> UpdatePatrolling(int id, PatrollingInfo patrolling)
        {
            if (id != patrolling.PatrollingId)
            {
                return BadRequest();
            }

            _dbContext.Entry(patrolling).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatrollingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<PatrollingInfo>> InsertPatrolling([FromBody] PatrollingInfo patrolling)
        {
            if (_dbContext.PatrollingInfos == null)
            {
                return Problem("Entity set 'ProPBTDbContext'  is null.");
            }
            _dbContext.PatrollingInfos.Add(patrolling);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("InsertPatrolling", new { id = patrolling.PatrollingId }, patrolling);
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatrolling(int id)
        {
            if (_dbContext.PatrollingInfos == null)
            {
                return NotFound();
            }
            var patrolling = await _dbContext.PatrollingInfos.FindAsync(id);
            if (patrolling == null)
            {
                return NotFound();
            }

            _dbContext.PatrollingInfos.Remove(patrolling);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private bool PatrollingExists(int id)
        {
            return (_dbContext.PatrollingInfos?.Any(e => e.PatrollingId == id)).GetValueOrDefault();
        }      
    }
}
