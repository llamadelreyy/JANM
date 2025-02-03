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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DepartmentController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly ILogger<DepartmentController> _logger;
        private readonly string _feature = "JABATAN";

        public DepartmentController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<DepartmentController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
        }

        #region ref_department
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ref_department>>> ListAll()
        {
            try
            {
                var data = await _dbContext.ref_departments.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.ref_departments.FirstOrDefaultAsync(x => x.dept_id == Id);

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ref_department InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region validation
                var existingDepartment = await _dbContext.ref_departments
                   .FirstOrDefaultAsync(d => d.dept_code == InputModel.dept_code && d.is_deleted == false);

                if (existingDepartment != null)
                {
                    return Error("", SystemMesg("COMMON", "DUPLICATE_DEPT_CODE", MessageTypeEnum.Error, "The dept_code already exists."));
                }
                #endregion

                #region store data
                ref_department ref_department = new ref_department
                {
                    dept_code = InputModel.dept_code,
                    dept_name = InputModel.dept_name,
                    dept_desc = InputModel.dept_desc,
                    is_deleted = InputModel.is_deleted,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.ref_departments.Add(ref_department);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    dept_code = ref_department.dept_code,
                    dept_name = ref_department.dept_name,
                    dept_desc = ref_department.dept_desc,
                    is_deleted = ref_department.is_deleted,
                    created_at = ref_department.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_department InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_departments.FirstOrDefaultAsync(x => x.dept_id == InputModel.dept_id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.dept_code))
                {
                    return Error("", SystemMesg(_feature, "DEPT_CODE", MessageTypeEnum.Error, string.Format("Ruangan Kod Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.dept_name))
                {
                    return Error("", SystemMesg(_feature, "DEPT_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Jabatan diperlukan")));
                }
                #endregion

                formField.dept_code = InputModel.dept_code;
                formField.dept_name = InputModel.dept_name;
                formField.dept_desc = InputModel.dept_desc;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _dbContext.ref_departments.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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
                var formField = await _dbContext.ref_departments.FirstOrDefaultAsync(x => x.dept_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ref_departments.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool ref_department_exists(int id)
        {
            return (_dbContext.ref_departments?.Any(e => e.dept_id == id)).GetValueOrDefault();
        }
        #endregion
    }
}
