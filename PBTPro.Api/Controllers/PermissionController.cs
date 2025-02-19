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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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

                var permissions = await _dbContext.permissions.Where(x => x.role_id == roleId)
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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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

                var rolesExist = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == InputModel.role_id);
                if (rolesExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var menusExist = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == InputModel.menu_id);
                if (menusExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
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

                return Ok(permission, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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

                var isExists = await _dbContext.permissions.FirstOrDefaultAsync(x => x.role_id == InputModel.role_id && x.menu_id == InputModel.menu_id && x.permission_id != Id);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }

                var rolesExist = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == InputModel.role_id);
                if (rolesExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var menusExist = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == InputModel.menu_id);
                if (menusExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
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

                return Ok(permission, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Remove(int Id)
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

                try
                {
                    _dbContext.permissions.Remove(permission);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    permission.is_deleted = true;
                    permission.modifier_id = runUserID;
                    permission.modified_at = DateTime.Now;

                    _dbContext.permissions.Update(permission);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));

                }
                return Ok(permission, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost("{Id}")]
        public async Task<IActionResult> BulkSaveByMenu(int Id, [FromBody] List<permission> InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (!InputModel.Any())
                {
                    return Error("", SystemMesg(_feature, "EMPTY_PAYLOAD", MessageTypeEnum.Error, string.Format("Tiada data untuk disimpan")));
                }

                if (InputModel.Any(x => x.menu_id != Id))
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
                }

                var menusExist = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == Id);
                if (menusExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
                }

                var roleIds = InputModel.Select(x => x.role_id).Distinct().ToList();
                var rolesExist = await _dbContext.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
                if (rolesExist.Count != roleIds.Count)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var duplicateRoleMenuPairs = InputModel
                    .GroupBy(x => new { x.role_id, x.menu_id })
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateRoleMenuPairs.Any())
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }

                var newPermissions = InputModel.Where(x => x.permission_id == 0).ToList();
                var existingPermissions = InputModel.Where(x => x.permission_id != 0).ToList();
                var dbPermission = await _dbContext.permissions//.Where(p => p.menu_id == Id)
                    .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                    .ToListAsync();

                if (existingPermissions.Any())
                {
                    var invalidRecId = dbPermission
                    .Where(p => existingPermissions
                    .Any(pair => pair.permission_id == p.permission_id && (pair.role_id != p.role_id || pair.menu_id != p.menu_id)))
                    .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                    .ToList();

                    if (invalidRecId.Any())
                    {
                        return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                    }
                }

                var duplicateRoleMenu = dbPermission
                .Where(p => InputModel
                .Any(pair => pair.permission_id == p.permission_id && (pair.role_id != p.role_id || pair.menu_id != p.menu_id)))
                .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                .ToList();

                if (duplicateRoleMenu.Any())
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }

                #endregion

                if (newPermissions.Any())
                {
                    foreach (var newPermission in newPermissions)
                    {
                        newPermission.creator_id = runUserID;
                        newPermission.created_at = DateTime.Now;
                    }

                    _dbContext.permissions.AddRange(newPermissions);
                }
                if (existingPermissions.Any())
                {
                    foreach (var existingPermission in existingPermissions)
                    {
                        existingPermission.modifier_id = runUserID;
                        existingPermission.modified_at = DateTime.Now;
                    }
                    _dbContext.permissions.UpdateRange(existingPermissions);
                }
                if (newPermissions.Any() || existingPermissions.Any())
                {
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "BULK_SAVE", MessageTypeEnum.Success, string.Format("Berjaya menyimpan kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost("{Id}")]
        public async Task<IActionResult> BulkSaveByRole(int Id, [FromBody] List<permission> InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (!InputModel.Any())
                {
                    return Error("", SystemMesg(_feature, "EMPTY_PAYLOAD", MessageTypeEnum.Error, string.Format("Tiada data untuk disimpan")));
                }

                if (InputModel.Any(x => x.role_id != Id))
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var rolesExist = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);
                if (rolesExist == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var menuIds = InputModel.Select(x => x.menu_id).Distinct().ToList();
                var menusExist = await _dbContext.menus.Where(r => menuIds.Contains(r.menu_id)).ToListAsync();
                if (menusExist.Count != menuIds.Count)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
                }

                var duplicateRoleMenuPairs = InputModel
                    .GroupBy(x => new { x.role_id, x.menu_id })
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateRoleMenuPairs.Any())
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }
                /* removing validate - will auto populate the existing data & new data based on db content
                var newPermissions = InputModel.Where(x => x.permission_id == 0).ToList();
                var existingPermissions = InputModel.Where(x => x.permission_id != 0).ToList();
                var dbPermission = await _dbContext.permissions//.Where(p => p.role_id == Id)
                    .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                    .ToListAsync();

                if (existingPermissions.Any())
                {
                    var invalidRecId = dbPermission
                    .Where(p => existingPermissions
                    .Any(pair => pair.permission_id == p.permission_id && (pair.role_id != p.role_id || pair.menu_id != p.menu_id)))
                    .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                    .ToList();

                    if (invalidRecId.Any())
                    {
                        return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                    }
                }

                var duplicateRoleMenu = dbPermission
                .Where(p => InputModel
                .Any(pair => pair.role_id == p.role_id && pair.menu_id == p.menu_id && p.permission_id != pair.permission_id))
                .Select(p => new { p.role_id, p.menu_id, p.permission_id })
                .ToList();

                if (duplicateRoleMenu.Any())
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }
                */
                #endregion

                #region Building Record
                var dbRolesPermissions = await _dbContext.permissions.Where(p => p.role_id == Id).AsNoTracking().ToListAsync();
                var existingPermissionsInDb = InputModel
                                    .Join(dbRolesPermissions,
                                          ip => new { ip.role_id, ip.menu_id },
                                          rp => new { rp.role_id, rp.menu_id },
                                          (ip, rp) => rp)
                                    .ToDictionary(rp => (rp.role_id, rp.menu_id), rp => rp);

                foreach (var inputPermission in InputModel)
                {                    
                    if (existingPermissionsInDb.TryGetValue((inputPermission.role_id, inputPermission.menu_id), out var existingPermission))
                    {
                        inputPermission.permission_id = existingPermission.permission_id;
                        inputPermission.creator_id = existingPermission.creator_id;
                        inputPermission.created_at = existingPermission.created_at;
                        inputPermission.is_deleted = false;
                        inputPermission.modifier_id = runUserID;
                        inputPermission.modified_at = DateTime.Now;
                    }
                    else
                    {
                        inputPermission.permission_id = 0;
                        inputPermission.is_deleted = false;
                        inputPermission.creator_id = runUserID;
                        inputPermission.created_at = DateTime.Now;
                    }
                }

                var removedPermissions = dbRolesPermissions.Where(rp => !InputModel.Any(ip => rp.role_id == ip.role_id && rp.menu_id == ip.menu_id)).ToList();
                var newPermissions = InputModel.Where(x => x.permission_id == 0).ToList();
                var existingPermissions = InputModel.Where(x => x.permission_id != 0).ToList();
                #endregion

                if (newPermissions.Any() || existingPermissions.Any())
                {
                    if (newPermissions.Any())
                    {
                        _dbContext.permissions.AddRange(newPermissions);
                    }
                    if (existingPermissions.Any())
                    {
                        _dbContext.permissions.UpdateRange(existingPermissions);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                if (removedPermissions.Any())
                {
                    try
                    {
                        _dbContext.permissions.RemoveRange(removedPermissions);
                    }
                    catch (Exception ex)
                    {
                        foreach (var removedPermission in removedPermissions)
                        {
                            removedPermission.modifier_id = runUserID;
                            removedPermission.modified_at = DateTime.Now;
                            removedPermission.is_deleted = true;
                        }
                        _dbContext.permissions.UpdateRange(removedPermissions);
                    }
                    finally
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                }

                return Ok("", SystemMesg(_feature, "BULK_SAVE", MessageTypeEnum.Success, string.Format("Berjaya menyimpan kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkSaveDynamic([FromBody] List<permission> InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (!InputModel.Any())
                {
                    return Error("", SystemMesg(_feature, "EMPTY_PAYLOAD", MessageTypeEnum.Error, string.Format("Tiada data untuk disimpan")));
                }

                if (InputModel.Any(x => x.role_id == null))
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                if (InputModel.Any(x => x.menu_id == null))
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var roleIds = InputModel.Select(x => x.role_id).Distinct().ToList();
                var rolesExist = await _dbContext.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
                if (rolesExist.Count != roleIds.Count)
                {
                    return Error("", SystemMesg(_feature, "INVALID_ROLE", MessageTypeEnum.Error, string.Format("Peranan tidak sah")));
                }

                var menuIds = InputModel.Select(x => x.menu_id).Distinct().ToList();
                var menusExist = await _dbContext.menus.Where(r => menuIds.Contains(r.menu_id)).ToListAsync();
                if (menusExist.Count != menuIds.Count)
                {
                    return Error("", SystemMesg(_feature, "INVALID_MENU", MessageTypeEnum.Error, string.Format("Menu tidak sah")));
                }

                var duplicateRoleMenuPairs = InputModel
                    .GroupBy(x => new { x.role_id, x.menu_id })
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateRoleMenuPairs.Any())
                {
                    return Error("", SystemMesg(_feature, "ROLE_MENU_ISEXISTS", MessageTypeEnum.Error, string.Format("Gabungan peranan dan menu telah wujud")));
                }
                #endregion

                #region Prepare Data
                var roleMenuPairs = InputModel.Where(im => im.permission_id == 0).Select(im => new { im.role_id, im.menu_id }).Distinct().ToList();

                var existingPermissionsInDb = (from rmp in roleMenuPairs
                                               join p in _dbContext.permissions.AsNoTracking()
                                                   on new { rmp.role_id, rmp.menu_id } equals new { p.role_id, p.menu_id }
                                               select p)
                               .ToDictionary(p => (p.role_id, p.menu_id), p => p);

                // var existingPermissionsInDb = _dbContext.permissions
                //.Where(p => roleMenuPairs.Any(rmp => rmp.role_id == p.role_id && rmp.menu_id == p.menu_id))
                //.ToList();

                //var existingPermissionsDict = existingPermissionsInDb
                //    .ToDictionary(p => (p.role_id, p.menu_id), p => p);

                foreach (var inputPermission in InputModel)
                {
                    if (inputPermission.permission_id == 0)
                    {
                        if (existingPermissionsInDb.TryGetValue((inputPermission.role_id, inputPermission.menu_id), out var existingPermission))
                        {
                            inputPermission.permission_id = existingPermission.permission_id;
                            inputPermission.creator_id = existingPermission.creator_id;
                            inputPermission.created_at = existingPermission.created_at;
                            inputPermission.modifier_id = runUserID;
                            inputPermission.modified_at = DateTime.Now;
                        }
                        else
                        {
                            inputPermission.creator_id = runUserID;
                            inputPermission.created_at = DateTime.Now;
                        }
                    }
                    else
                    {
                        inputPermission.modifier_id = runUserID;
                        inputPermission.modified_at = DateTime.Now;
                    }
                }

                var newPermissions = InputModel.Where(x => x.permission_id == 0).ToList();
                var existingPermissions = InputModel.Where(x => x.permission_id != 0).ToList();
                #endregion

                if (newPermissions.Any() || existingPermissions.Any())
                {
                    if (newPermissions.Any())
                    {
                        _dbContext.permissions.AddRange(newPermissions);
                    }
                    if (existingPermissions.Any())
                    {
                        _dbContext.permissions.UpdateRange(existingPermissions);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "BULK_SAVE", MessageTypeEnum.Success, string.Format("Berjaya menyimpan kebenaran akses")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
