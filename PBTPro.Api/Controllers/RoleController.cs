/*
Project: PBT Pro
Description: Permission controller
Author: Farhana
Date: January 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
-
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RoleController : IBaseController
    {
        private readonly ILogger<RoleController> _logger;
        private readonly string _feature = "ROLE";

        public RoleController(PBTProDbContext dbContext, ILogger<RoleController> logger) : base(dbContext)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var result = await _dbContext.roles.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (result.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] role InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var isExists = await _dbContext.roles.FirstOrDefaultAsync(x => x.role_id == InputModel.role_id);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }
                #endregion

                role role = new role
                {
                    role_id = InputModel.role_id,
                    role_name = InputModel.role_name,
                    role_desc = InputModel.role_desc,
                    is_default_role = InputModel.is_default_role,
                    is_tenant = InputModel.is_tenant,                    
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.roles.Add(role);
                await _dbContext.SaveChangesAsync();

                return Ok(role, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] role InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var role = await _dbContext.roles.FirstOrDefaultAsync(x => x.role_id == Id);
                if (role == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                role.role_name = InputModel.role_name;
                role.role_desc = InputModel.role_desc;
                role.is_default_role = InputModel.is_default_role;
                role.is_tenant = InputModel.is_tenant;
                role.modifier_id = runUserID;
                role.modified_at = DateTime.Now;

                _dbContext.roles.Update(role);
                await _dbContext.SaveChangesAsync();

                return Ok(role, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {

                #region Validation
                var role = await _dbContext.roles.FirstOrDefaultAsync(x => x.role_id == Id);
                if (role == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.roles.Remove(role);
                await _dbContext.SaveChangesAsync();

                return Ok(role, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var role = await _dbContext.roles.FirstOrDefaultAsync(x => x.role_id == Id);

                if (role == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(role, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
