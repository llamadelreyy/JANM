/*
Project: PBT Pro
Description: Department API controller to handle department Form Field
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
    public class DepartmentController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Department";
        private readonly PBTProDbContext _dbContext;
        private List<department_info> _Department { get; set; }

        public DepartmentController(PBTProDbContext dbContext, ILogger<DepartmentController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<department_info>>> ListDepartment()
        {
            if (_dbContext.department_infos == null)
            {
                return NotFound();
            }
            return await _dbContext.department_infos.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<department_info>> RetrieveDepartment(int id)
        {
            if (_dbContext.department_infos == null)
            {
                return NotFound();
            }
            var Department = await _dbContext.department_infos.FindAsync(id);

            if (Department == null)
            {
                return NotFound();
            }

            return Department;
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, department_info Department)
        {
            if (id != Department.dept_id)
            {
                return BadRequest();
            }

            _dbContext.Entry(Department).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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
        public async Task<ActionResult<department_info>> InsertDepartment([FromBody] department_info Department)
        {         
            if (_dbContext.department_infos == null)
            {
                return Problem("Entity set 'ProPBTDbContext'  is null.");
            }
            _dbContext.department_infos.Add(Department);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("InsertDepartment", new { id = Department.dept_id }, Department);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (_dbContext.department_infos == null)
            {
                return NotFound();
            }
            var Department = await _dbContext.department_infos.FindAsync(id);
            if (Department == null)
            {
                return NotFound();
            }

            _dbContext.department_infos.Remove(Department);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private bool DepartmentExists(int id)
        {
            return (_dbContext.department_infos?.Any(e => e.dept_id == id)).GetValueOrDefault();
        }      
    }
}
