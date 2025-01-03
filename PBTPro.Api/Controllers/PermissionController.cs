/*
Project: PBT Pro
Description: Permission controller
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
02/01/2025 - initial create
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
    public class PermissionController : IBaseController
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly string _feature = "PERMISSION";

        public PermissionController(PBTProDbContext dbContext, ILogger<PermissionController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var permissions = await _dbContext.permissions.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (permissions.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(permissions, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListByRole(int roleId)
        {
            try
            {
                if (roleId == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLEID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var permissions = await _dbContext.permissions.Where(x=> x.role_id == roleId)
                      .AsNoTracking()
                      .ToListAsync();

                if (permissions.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(permissions, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListByMenu(int menuId)
        {
            try
            {
                if (menuId == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENUID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var permissions = await _dbContext.permissions.Where(x => x.menu_id == menuId)
                      .AsNoTracking()
                      .ToListAsync();

                if (permissions.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(permissions, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] permission InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var isExists = await _dbContext.permissions.FirstOrDefaultAsync(x => x.role_id == InputModel.role_id && x.menu_id == InputModel.menu_id);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }
                #endregion

                permission permission = new permission
                {
                    role_id = InputModel.role_id,
                    menu_id = InputModel.menu_id,
                    can_view = InputModel.can_view,
                    can_add = InputModel.can_add,
                    can_delete = InputModel.can_delete,
                    can_edit = InputModel.can_edit,
                    can_print = InputModel.can_print,
                    can_download = InputModel.can_download,
                    can_upload = InputModel.can_upload,
                    can_execute = InputModel.can_execute,
                    can_authorize = InputModel.can_authorize,
                    can_view_sensitive = InputModel.can_view_sensitive,
                    can_export_data = InputModel.can_export_data,
                    can_import_data = InputModel.can_import_data,
                    can_approve_changes = InputModel.can_approve_changes,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.permissions.Add(permission);
                await _dbContext.SaveChangesAsync();

                return Ok(permission, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] permission InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var permission = await _dbContext.permissions.FirstOrDefaultAsync(x => x.permission_id == Id);
                if (permission == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                permission.can_view = InputModel.can_view;
                permission.can_add = InputModel.can_add;
                permission.can_delete = InputModel.can_delete;
                permission.can_edit = InputModel.can_edit;
                permission.can_print = InputModel.can_print;
                permission.can_download = InputModel.can_download;
                permission.can_upload = InputModel.can_upload;
                permission.can_execute = InputModel.can_execute;
                permission.can_authorize = InputModel.can_authorize;
                permission.can_view_sensitive = InputModel.can_view_sensitive;
                permission.can_export_data = InputModel.can_export_data;
                permission.can_import_data = InputModel.can_import_data;
                permission.can_approve_changes = InputModel.can_approve_changes;
                permission.modifier_id = runUserID;
                permission.modified_at = DateTime.Now;

                _dbContext.permissions.Update(permission);
                await _dbContext.SaveChangesAsync();

                return Ok(permission, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Remove(int Id)
        {
            try
            {

                #region Validation
                var permission = await _dbContext.permissions.FirstOrDefaultAsync(x => x.permission_id == Id);
                if (permission == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();

                return Ok(permission, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

    }
}
