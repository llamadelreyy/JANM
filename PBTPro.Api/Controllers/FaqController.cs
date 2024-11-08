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
    public class FaqController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<FaqController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Faq";
        private readonly PBTProDbContext _dbContext;
        private List<TbFaq> _Faq { get; set; }

        public FaqController(PBTProDbContext dbContext, ILogger<FaqController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TbFaq>>> ListFaq()
        {
            if (_dbContext.TbFaqs == null)
            {
                return NotFound();
            }
            return await _dbContext.TbFaqs.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TbFaq>> RetrieveFaq(int id)
        {
            if (_dbContext.TbFaqs == null)
            {
                return NotFound();
            }
            var faq = await _dbContext.TbFaqs.FindAsync(id);

            if (faq == null)
            {
                return NotFound();
            }

            return faq;
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaq(int id, TbFaq faq)
        {
            if (id != faq.Faqid)
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

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<TbFaq>> InsertFaq([FromBody] string faqs = "")
        {
            TbFaq faq = JsonConvert.DeserializeObject<TbFaq>(faqs);

            if (_dbContext.TbFaqs == null)
            {
                return Problem("Entity set 'ProPBTDbContext'  is null.");
            }
            _dbContext.TbFaqs.Add(faq);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("InsertFaq", new { id = faq.Faqid }, faqs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            if (_dbContext.TbFaqs == null)
            {
                return NotFound();
            }
            var faq = await _dbContext.TbFaqs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            _dbContext.TbFaqs.Remove(faq);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool FaqExists(int id)
        {
            return (_dbContext.TbFaqs?.Any(e => e.Faqid == id)).GetValueOrDefault();
        }      
    }
}
