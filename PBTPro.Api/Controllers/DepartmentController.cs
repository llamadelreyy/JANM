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
        private string LoggerName = "administrator";
        private readonly string _feature = "JABATAN";

        public DepartmentController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<DepartmentController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<department_info>>> ListDepartment()
        {            
            try
            {
                var data = await _dbContext.department_infos.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RetrieveDepartment(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.department_infos.FirstOrDefaultAsync(x => x.dept_id == Id);

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
        public async Task<IActionResult> InsertDepartment([FromBody] department_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                department_info department_infos = new department_info
                {
                    dept_code = InputModel.dept_code,
                    dept_depart_name = InputModel.dept_depart_name,
                    dept_description = InputModel.dept_description,
                    dept_status = InputModel.dept_status,
                    created_by = runUserID,
                    created_date = DateTime.Now,
                };

                _dbContext.department_infos.Add(department_infos);
                await _dbContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    dept_code = department_infos.dept_code,
                    dept_depart_name = department_infos.dept_depart_name,
                    dept_description = department_infos.dept_description,
                    dept_status = department_infos.dept_status,
                    created_date = department_infos.created_date
                };
                return Ok(result, SystemMesg(_feature, "CREATE_PATROL_SCHEDULER", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> PutDepartment(int Id, [FromBody] department_info InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.department_infos.FirstOrDefaultAsync(x => x.dept_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.dept_code))
                {
                    return Error("", SystemMesg(_feature, "DEPT_CODE", MessageTypeEnum.Error, string.Format("Ruangan Kod Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.dept_depart_name))
                {
                    return Error("", SystemMesg(_feature, "DEPT_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Jabatan diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.dept_status))
                {
                    return Error("", SystemMesg(_feature, "DEPT_STATUS", MessageTypeEnum.Error, string.Format("Ruangan Status Jabatan diperlukan")));
                }
                #endregion

                formField.dept_code = InputModel.dept_code;
                formField.dept_depart_name = InputModel.dept_depart_name;
                formField.dept_description = InputModel.dept_description;
                formField.dept_status = InputModel.dept_status;

                formField.updated_by = runUserID;
                formField.updated_date = DateTime.Now;

                _dbContext.department_infos.Update(formField);
                await _dbContext.SaveChangesAsync();
                
                return Ok(formField, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDepartment(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.department_infos.FirstOrDefaultAsync(x => x.dept_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.department_infos.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool DepartmentExists(int id)
        {
            return (_dbContext.department_infos?.Any(e => e.dept_id == id)).GetValueOrDefault();
        }

        #region unused
        //[AllowAnonymous]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<department_info>> RetrieveDepartment(int id)
        //{
        //    if (_dbContext.department_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    var Department = await _dbContext.department_infos.FindAsync(id);

        //    if (Department == null)
        //    {
        //        return NotFound();
        //    }

        //    return Department;
        //}

        //[AllowAnonymous]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateDepartment(int id, department_info Department)
        //{
        //    if (id != Department.dept_id)
        //    {
        //        return BadRequest();
        //    }

        //    _dbContext.Entry(Department).State = EntityState.Modified;

        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DepartmentExists(id))
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




        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDepartment(int id)
        //{
        //    if (_dbContext.department_infos == null)
        //    {
        //        return NotFound();
        //    }
        //    var Department = await _dbContext.department_infos.FindAsync(id);
        //    if (Department == null)
        //    {
        //        return NotFound();
        //    }

        //    _dbContext.department_infos.Remove(Department);
        //    await _dbContext.SaveChangesAsync();

        //    return Ok();
        //}

        #endregion
    }
}
