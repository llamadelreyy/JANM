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
10/01/2025 - remove allow anonymous for certain method
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FaqController> _logger;
        private readonly string _feature = "SOALAN_LAZIM";

        public FaqController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<FaqController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ref_faq>>> ListAll()
        {
            try
            {
                var data = await _dbContext.ref_faqs.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.ref_faqs.FirstOrDefaultAsync(x => x.faq_id == Id);

                if (parFormfield == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                return Ok(parFormfield, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ref_faq InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_faqs.FirstOrDefaultAsync();
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_category))
                {
                    return Error("", SystemMesg(_feature, "KATEGORI", MessageTypeEnum.Error, string.Format("Ruangan kategoru soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_question))
                {
                    return Error("", SystemMesg(_feature, "SOALAN", MessageTypeEnum.Error, string.Format("Ruangan soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_answer))
                {
                    return Error("", SystemMesg(_feature, "JAWAPAN", MessageTypeEnum.Error, string.Format("Ruangan jawapan soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_status))
                {
                    return Error("", SystemMesg(_feature, "STATUS", MessageTypeEnum.Error, string.Format("Ruangan status soalan lazim diperlukan")));
                }
                #endregion

                #region store data
                ref_faq faq_info = new ref_faq
                {
                    faq_status = InputModel.faq_status,
                    faq_answer = InputModel.faq_answer,
                    faq_category = InputModel.faq_category,
                    faq_question = InputModel.faq_question,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.ref_faqs.Add(faq_info);
                await _dbContext.SaveChangesAsync();

                #endregion
               
                return Ok(faq_info, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya tambah data.")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_faq InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_faqs.FirstOrDefaultAsync(x => x.faq_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_category))
                {
                    return Error("", SystemMesg(_feature, "KATEGORI", MessageTypeEnum.Error, string.Format("Ruangan kategori soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_question))
                {
                    return Error("", SystemMesg(_feature, "SOALAN", MessageTypeEnum.Error, string.Format("Ruangan soalan soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_answer))
                {
                    return Error("", SystemMesg(_feature, "JAWAPAN", MessageTypeEnum.Error, string.Format("Ruangan jawapan soalan lazim diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.faq_status))
                {
                    return Error("", SystemMesg(_feature, "STATUS", MessageTypeEnum.Error, string.Format("Ruangan satus soalan lazim diperlukan")));
                }
                #endregion

                formField.faq_category = InputModel.faq_category;
                formField.faq_question = InputModel.faq_question;
                formField.faq_answer = InputModel.faq_answer;
                formField.faq_status = InputModel.faq_status;

                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _dbContext.ref_faqs.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_faqs.FirstOrDefaultAsync(x => x.faq_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ref_faqs.Remove(formField);
                await _dbContext.SaveChangesAsync();
                
                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool FaqExists(int id)
        {
            return (_dbContext.ref_faqs?.Any(e => e.faq_id == id)).GetValueOrDefault();
        }

       
    }
}
