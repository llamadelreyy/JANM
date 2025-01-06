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
using NetTopologySuite.GeometriesGraph;
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
    public class UnitController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private string LoggerName = "administrator";
        private readonly string _feature = "UNIT";

        public UnitController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<UnitController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ref_unit>>> ListAll()
        {            
            try
            {
                var data = await (from unit in _dbContext.ref_units
                                  join dept in _dbContext.ref_departments
                                  on unit.dept_id equals dept.dept_id
                                  join div in _dbContext.ref_divisions
                                  on unit.div_id equals div.div_id
                                  select new
                                  {
                                      unit_id = unit.unit_id,
                                      dept_id = unit.dept_id,
                                      unit_code = unit.unit_code,
                                      unit_name = unit.unit_name,
                                      unit_desc = unit.unit_desc,
                                      created_at = unit.created_at,
                                      creator_id = unit.creator_id,
                                      dept_name = dept.dept_name,
                                      div_name = div.div_name,
                                      div_id = div.div_id

                                  }).ToListAsync();

                if (data == null)
                {
                    var data1 = await _dbContext.ref_units.AsNoTracking().ToListAsync();
                    return Ok(data1, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
                }

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
                var parFormfield = await _dbContext.ref_units.FirstOrDefaultAsync(x => x.unit_id == Id);

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
        public async Task<IActionResult> Add([FromBody] ref_unit InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region validation
                var existingUnit = await _dbContext.ref_units
                   .FirstOrDefaultAsync(d => d.unit_code == InputModel.unit_code && d.dept_name == InputModel.dept_name && d.div_name == InputModel.div_name && d.is_deleted == false);

                if (existingUnit != null)
                {
                    return Error("",SystemMesg("COMMON", "DUPLICATE_UNIT_CODE_DIV_NAME_DEPT_NAME", MessageTypeEnum.Error, "The unit_code with the same dept_name already exists."));
                }
                #endregion

                #region store data
                ref_unit division_infos = new ref_unit
                {
                    unit_code = InputModel.unit_code,
                    unit_name = InputModel.unit_name,
                    unit_desc = InputModel.unit_desc,
                    dept_id = InputModel.dept_id,
                    dept_name = InputModel.dept_name,
                    div_id = InputModel.div_id,
                    div_name = InputModel.div_name,
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.ref_units.Add(division_infos);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    unit_code = division_infos.unit_code,
                    unit_name = division_infos.unit_name,
                    unit_desc = division_infos.unit_desc,
                    dept_id = division_infos.dept_id,
                    dept_name = division_infos.dept_name,
                    div_id = division_infos.div_id,
                    div_name = division_infos.div_name,
                    is_deleted = division_infos.is_deleted,
                    created_at = division_infos.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_unit InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ref_units.FirstOrDefaultAsync(x => x.unit_id == InputModel.unit_id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.unit_code))
                {
                    return Error("", SystemMesg(_feature, "UNIT_CODE", MessageTypeEnum.Error, string.Format("Ruangan Kod Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.unit_name))
                {
                    return Error("", SystemMesg(_feature, "DIV_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Jabatan diperlukan")));
                }
                
                #endregion

                formField.unit_code = InputModel.unit_code;
                formField.unit_name = InputModel.unit_name;
                formField.unit_desc = InputModel.unit_desc;
                formField.dept_id = InputModel.dept_id;
                formField.dept_name = InputModel.dept_name;
                formField.div_id = InputModel.div_id;
                formField.div_name = InputModel.div_name;
                formField.is_deleted = InputModel.is_deleted;

                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _dbContext.ref_units.Update(formField);
                await _dbContext.SaveChangesAsync();
                
                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
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
                var formField = await _dbContext.ref_units.FirstOrDefaultAsync(x => x.unit_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ref_units.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool UnitExists(int id)
        {
            return (_dbContext.ref_units?.Any(e => e.unit_id == id)).GetValueOrDefault();
        }        

    }
}
