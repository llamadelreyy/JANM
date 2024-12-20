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

using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FaqController : IBaseController
    {        
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private string LoggerName = "administrator";
        private readonly string _feature = "SOALAN_LAZIM";

        public FaqController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<FaqController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<faq_info>>> ListAll()
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
        public async Task<IActionResult> ViewDetail(int Id)
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
        public async Task<IActionResult> Add([FromBody] faq_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.faq_infos.FirstOrDefaultAsync();
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
               
                return Ok(faq_info, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] faq_info InputModel)
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

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
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
                
                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
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
    }
}
