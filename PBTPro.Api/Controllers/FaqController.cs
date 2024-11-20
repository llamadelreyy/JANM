/*
Project: PBT Pro
Description: FAQ API controller to handle FAQ Form Field
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
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FaqController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<FaqController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Faq";
        private readonly PBTProDbContext _dbContext;
        private List<faq_info> _Faq { get; set; }

        public FaqController(PBTProDbContext dbContext, ILogger<FaqController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<faq_info>>> ListFaq()
        {
            if (_dbContext.faq_infos == null)
            {
                return NotFound();
            }
            return await _dbContext.faq_infos.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<faq_info>> RetrieveFaq(int id)
        {
            if (_dbContext.faq_infos == null)
            {
                return NotFound();
            }
            var faq = await _dbContext.faq_infos.FindAsync(id);

            if (faq == null)
            {
                return NotFound();
            }

            return faq;
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<ActionResult<faq_info>> UpdateFaq(int id, faq_info faq)
        {
            if (id != faq.faq_id)
            {
                return BadRequest();
            }

            _dbContext.Entry(faq).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaqExists(id))
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
        public async Task<ActionResult<faq_info>> InsertFaq([FromBody] faq_info faq)
        {
            //FaqInfo faq = JsonConvert.DeserializeObject<FaqInfo>(faqs);

            if (_dbContext.faq_infos == null)
            {
                return Problem("Entity set 'ProPBTDbContext'  is null.");
            }
            _dbContext.faq_infos.Add(faq);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("InsertFaq", new { id = faq.faq_id }, faq);
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            if (_dbContext.faq_infos == null)
            {
                return NotFound();
            }
            var faq = await _dbContext.faq_infos.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            _dbContext.faq_infos.Remove(faq);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private bool FaqExists(int id)
        {
            return (_dbContext.faq_infos?.Any(e => e.faq_id == id)).GetValueOrDefault();
        }      
    }
}
