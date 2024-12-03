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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FaqController : IBaseController
    {
        private readonly ILogger<FaqController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PBTProDbContext _dbContext;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "SOALAN_LAZIM";

        private List<faq_info> _Faq { get; set; }

        public FaqController(PBTProDbContext dbContext, ILogger<FaqController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<faq_info>>> ListFaq()
        {
            try
            {
                var data = await _dbContext.faq_infos.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RetrieveFaq(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.faq_infos.FirstOrDefaultAsync(x => x.faq_id == Id);

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
        [HttpPost]
        public async Task<IActionResult> InsertFaq([FromBody] faq_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);

                #region store data
                faq_info faq_info = new faq_info
                {
                    faq_status = InputModel.faq_status,
                    faq_answer = InputModel.faq_answer,
                    faq_category = InputModel.faq_category,
                    faq_question = InputModel.faq_question,
                    created_by = runUserID,
                    created_date = DateTime.Now,
                };

                _dbContext.faq_infos.Add(faq_info);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    faq_category = faq_info.faq_category,
                    faq_status = faq_info.faq_status,
                    faq_answer = faq_info.faq_answer,
                    faq_question = faq_info.faq_question,
                    created_date = faq_info.created_date
                };

                return Ok(result, SystemMesg(_feature, "CREATE_FAQ", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateFaq(int Id, [FromBody] faq_info InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.faq_infos.FirstOrDefaultAsync(x => x.faq_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.faq_category))
                {
                    return Error("", SystemMesg(_feature, "DEPT_CODE", MessageTypeEnum.Error, string.Format("Ruangan Kod Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_question))
                {
                    return Error("", SystemMesg(_feature, "DEPT_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_answer))
                {
                    return Error("", SystemMesg(_feature, "DEPT_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_status))
                {
                    return Error("", SystemMesg(_feature, "DEPT_STATUS", MessageTypeEnum.Error, string.Format("Ruangan Status Jabatan diperlukan")));
                }
                #endregion

                formField.faq_category = InputModel.faq_category;
                formField.faq_question = InputModel.faq_question;
                formField.faq_answer = InputModel.faq_answer;
                formField.faq_status = InputModel.faq_status;

                formField.updated_by = runUserID;
                formField.updated_date = DateTime.Now;

                _dbContext.faq_infos.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE_FAQ", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteFaq(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.faq_infos.FirstOrDefaultAsync(x => x.faq_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.faq_infos.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "DELETE_FAQ", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool FaqExists(int id)
        {
            return (_dbContext.faq_infos?.Any(e => e.faq_id == id)).GetValueOrDefault();
        }

        #region unused
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<faq_info>>> ListFaq()
        //{
        //    if (_dbContext.faq_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    return await _dbContext.faq_infos.ToListAsync();
        //}

        //[AllowAnonymous]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<faq_info>> RetrieveFaq(int id)
        //{
        //    if (_dbContext.faq_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    var faq = await _dbContext.faq_infos.FindAsync(id);

        //    if (faq == null)
        //    {
        //        return NotFound();
        //    }

        //    return faq;
        //}

        //[AllowAnonymous]
        //[HttpPut("{id}")]
        //public async Task<ActionResult<faq_info>> UpdateFaq(int id, faq_info faq)
        //{
        //    if (id != faq.faq_id)
        //    {
        //        return BadRequest();
        //    }

        //    _dbContext.Entry(faq).State = EntityState.Modified;

        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FaqExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return Ok();
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult<faq_info>> InsertFaq([FromBody] faq_info faq)
        //{
        //    //FaqInfo faq = JsonConvert.DeserializeObject<FaqInfo>(faqs);

        //    if (_dbContext.faq_infos == null)
        //    {
        //        return Problem("Entity set 'ProPBTDbContext'  is null.");
        //    }
        //    _dbContext.faq_infos.Add(faq);
        //    await _dbContext.SaveChangesAsync();

        //    return CreatedAtAction("InsertFaq", new { id = faq.faq_id }, faq);
        //}

        //[AllowAnonymous]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFaq(int id)
        //{
        //    if (_dbContext.faq_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    var faq = await _dbContext.faq_infos.FindAsync(id);
        //    if (faq == null)
        //    {
        //        return NotFound();
        //    }

        //    _dbContext.faq_infos.Remove(faq);
        //    await _dbContext.SaveChangesAsync();

        //    return Ok();
        //}
        #endregion
    }
}
