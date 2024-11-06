using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBTPro.Shared.Models;
using System.Collections.Generic;

namespace PBTPro.Api.Services
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public partial class ActionSettingsAPI : ControllerBase, IDisposable
    {
        private bool disposed = false;
        private readonly PbtproDbContext _dbContext;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                }

                // Dispose unmanaged resources here

                disposed = true;
            }
        }
        public IConfiguration _configuration { get; }
        public ActionSettingsAPI(IConfiguration configuration, PbtproDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shared.Models.Action>>> GetAllActionSetting()
        {
            if (_dbContext.Actions == null)
                return NotFound();

            return await _dbContext.Actions.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Shared.Models.Action>> GetIdActionSetting(int id)
        {
            if (_dbContext.Actions == null)
            {
                return NotFound();
            }
            var action = await _dbContext.Actions.FindAsync(id);

            if (action == null)
            {
                return NotFound();
            }
            return action;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActionSetting(int id)
        {
            if (_dbContext.Actions == null)
            {
                return NotFound();
            }
            var action = await _dbContext.Actions.FindAsync(id);
            if (action == null)
            {
                return NotFound();
            }
            _dbContext.Actions.Remove(action);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<Shared.Models.Action>> PostActionSetting([FromBody] string actions = "")
        {
            Shared.Models.Action action = JsonConvert.DeserializeObject<Shared.Models.Action>(actions);

            if (_dbContext.Actions == null)
            {
                return Problem("Entity set 'CADbContextv3.Cofaq'  is null.");
            }
            _dbContext.Actions.Add(action);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("PostActionSetting", new { id = action.Actionid }, action);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCofaq(int id, Shared.Models.Action action)
        {
            if (id != action.Actionid)
            {
                return BadRequest();
            }
            _dbContext.Entry(action).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionSettingExists(id))
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
        private bool ActionSettingExists(int id)
        {
            return (_dbContext.Actions?.Any(e => e.Actionid == id)).GetValueOrDefault();
        }
    }
}
