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
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DivisionController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly ILogger<DivisionController> _logger;
        private readonly string _feature = "UNIT";

        public DivisionController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<DivisionController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ref_division>>> ListAll()
        {
            try
            {
                var data = await (from division in _dbContext.ref_divisions
                                  join dept in _dbContext.ref_departments
                                  on division.dept_id equals dept.dept_id
                                  select new
                                  {
                                      div_id = division.div_id,
                                      dept_id = division.dept_id,
                                      div_code = division.div_code,
                                      div_name = division.div_name,
                                      div_desc = division.div_desc,
                                      created_at = division.created_at,
                                      creator_id = division.creator_id,
                                      dept_name = dept.dept_name,

                                  }).ToListAsync();

                if (data == null)
                {
                    var data1 = await _dbContext.ref_divisions.AsNoTracking().ToListAsync();
                    return Ok(data1, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }

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
                var parFormfield = await _dbContext.ref_divisions.FirstOrDefaultAsync(x => x.div_id == Id);

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
        public async Task<IActionResult> Add([FromBody] ref_division InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region validation
                var existingDivision = await _dbContext.ref_divisions
                   .FirstOrDefaultAsync(d => d.div_code == InputModel.div_code && d.dept_name == InputModel.dept_name && d.is_deleted == false);

                if (existingDivision != null)
                {
                    return Error("",SystemMesg("COMMON", "DUPLICATE_DIV_CODE_DEPT_NAME", MessageTypeEnum.Error, "Kod Seksyen sama dengan kod jabatan telah wujud."));
                }
                #endregion

                #region store data               
                ref_division division_infos = new ref_division
                {
                    div_code = InputModel.div_code,
                    div_name = InputModel.div_name,
                    div_desc = InputModel.div_desc,
                    dept_id = InputModel.dept_id,
                    dept_name = InputModel.dept_name,
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.ref_divisions.Add(division_infos);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    div_code = division_infos.div_code,
                    div_name = division_infos.div_name,
                    div_desc = division_infos.div_desc,
                    dept_id = division_infos.dept_id,
                    dept_name = division_infos.dept_name,
                    is_deleted = division_infos.is_deleted,
                    created_at = division_infos.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya tambah data.")));
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // 23505 is the unique violation error code
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Kod Seksyen sama dengan kod jabatan telah wujud.")));// BadRequest("The combination of div_code and dept_name already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_division InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_divisions.FirstOrDefaultAsync(x => x.div_id == InputModel.div_id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.div_code))
                {
                    return Error("", SystemMesg(_feature, "DIV_CODE", MessageTypeEnum.Error, string.Format("Ruangan Kod seksyen diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.div_name))
                {
                    return Error("", SystemMesg(_feature, "DIV_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama seksyen diperlukan")));
                }

                #endregion

                formField.div_code = InputModel.div_code;
                formField.div_name = InputModel.div_name;
                formField.div_desc = InputModel.div_desc;
                formField.dept_id = InputModel.dept_id;
                formField.dept_name = InputModel.dept_name;
                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _dbContext.ref_divisions.Update(formField);
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
                var formField = await _dbContext.ref_divisions.FirstOrDefaultAsync(x => x.div_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ref_divisions.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool DivisionExists(int id)
        {
            return (_dbContext.ref_divisions?.Any(e => e.div_id == id)).GetValueOrDefault();
        }

    }
}
