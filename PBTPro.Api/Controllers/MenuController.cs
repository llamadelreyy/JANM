/*
Project: PBT Pro
Description: Menu controller
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
02/01/2025 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Collections.Generic;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class MenuController : IBaseController
    {
        private readonly ILogger<MenuController> _logger;
        private readonly string _feature = "MENU";

        public MenuController(PBTProDbContext dbContext, ILogger<MenuController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var menus = await _dbContext.menus.Where(x => x.is_deleted != true).OrderBy(x => x.menu_sequence).ThenBy(x => x.menu_name).AsNoTracking().ToListAsync();

                if (menus.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(menus, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByAuth()
        {
            try
            {
                List<menu> menus = new List<menu>();
                int runUserID = await getDefRunUserId();
                if (runUserID > 0)
                {
                    menus = await (from ur in _dbContext.UserRoles
                                       join p in _dbContext.permissions on ur.RoleId equals p.role_id
                                       join m in _dbContext.menus on p.menu_id equals m.menu_id
                                       where ur.UserId == runUserID
                                   select m)
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    //Public Menu
                    menus = await(from p in _dbContext.permissions
                           join m in _dbContext.menus on p.menu_id equals m.menu_id
                           where p.role_id == 0
                           select m)
                    .AsNoTracking()
                    .ToListAsync();
                }

                List<menu> uniqueMenus = menus.GroupBy(m => m.menu_id)
                       .Select(g => g.First())
                       .OrderBy(x => x.menu_sequence)
                       .ThenBy(x => x.menu_name)
                       .ToList();

                return Ok(uniqueMenus, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
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

                var menus = await (from p in _dbContext.permissions
                                   join m in _dbContext.menus on p.menu_id equals m.menu_id
                                   where p.role_id == roleId
                                   select m)
                      .AsNoTracking()
                      .ToListAsync();

                if (menus.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(menus, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemByAuth()
        {
            try
            {
                List<menu> menus = new List<menu>();
                int runUserID = await getDefRunUserId();
                if (runUserID > 0)
                {
                    menus = await (from ur in _dbContext.UserRoles
                                   join p in _dbContext.permissions on ur.RoleId equals p.role_id
                                   join m in _dbContext.menus on p.menu_id equals m.menu_id
                                   where m.is_deleted != true && p.can_view == true && ur.UserId == runUserID
                                   select m)
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    //Public Menu
                    menus = await (from p in _dbContext.permissions
                                   join m in _dbContext.menus on p.menu_id equals m.menu_id
                                   where m.is_deleted != true && p.can_view == true && p.role_id == 0
                                   select m)
                    .AsNoTracking()
                    .ToListAsync();
                }

                menus = menus.GroupBy(m => m.menu_id)
                       .Select(g => g.First())
                       .OrderBy(x => x.menu_sequence)
                       .ThenBy(x => x.menu_name)
                       .ToList();

                List<MenuViewItem> uniqueMenus = new List<MenuViewItem>();

                foreach(var menu in menus.Where(x=>x.parent_id == 0).ToList())
                {
                    string CssClass = "singleNodesClass";
                    var SubMenu = GetSubMenu(menu.menu_id, menus);
                    var uniqueMenu = new MenuViewItem
                    {
                        Text = menu.menu_name,
                        NavigateUrl = menu.menu_path,
                        IconUrl = menu.icon_path,
                        BadgeText = null,
                        SubMenu = SubMenu
                    };

                    uniqueMenus.Add(uniqueMenu);
                }

                return Ok(uniqueMenus, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        [HttpGet("{Id}")]
        //[Route("GetDetail")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var menu = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == Id);

                if (menu == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(menu, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        //[Route("Create")]
        public async Task<IActionResult> Add([FromBody] menu InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.menu_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                menu menu = new menu
                {
                    module_id = InputModel.module_id,
                    parent_id = InputModel.parent_id,
                    menu_name = InputModel.menu_name,
                    menu_sequence = InputModel.menu_sequence,
                    icon_path = InputModel.icon_path,
                    is_tenant = InputModel.is_tenant,
                    menu_path = InputModel.menu_path,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.menus.Add(menu);
                await _dbContext.SaveChangesAsync();

                return Ok(menu, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] menu InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var menu = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == Id);
                if (menu == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.menu_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                menu.module_id = InputModel.module_id;
                menu.parent_id = InputModel.parent_id;
                menu.menu_name = InputModel.menu_name;
                menu.menu_sequence = InputModel.menu_sequence;
                menu.icon_path = InputModel.icon_path;
                menu.is_tenant = InputModel.is_tenant;
                menu.menu_path = InputModel.menu_path;
                menu.modifier_id = runUserID;
                menu.modified_at = DateTime.Now;

                _dbContext.menus.Update(menu);
                await _dbContext.SaveChangesAsync();

                return Ok(menu, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
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
                var menu = await _dbContext.menus.FirstOrDefaultAsync(x => x.menu_id == Id);
                if (menu == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _dbContext.menus.Remove(menu);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    menu.is_deleted = true;
                    menu.modifier_id = runUserID;
                    menu.modified_at = DateTime.Now;

                    _dbContext.menus.Update(menu);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(menu, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        #region Private Logic
        private List<MenuViewItem>? GetSubMenu(int MenuId, List<menu> menus)
        {
            List<MenuViewItem> SubMenu = new List<MenuViewItem>();
            try
            {
                SubMenu = menus.Where(x => x.parent_id == MenuId).Select(x => new MenuViewItem 
                {
                    Text = x.menu_name,
                    NavigateUrl = x.menu_path,
                    IconUrl = x.icon_path,
                    BadgeText = null,
                    SubMenu = GetSubMenu(x.menu_id, menus)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }
            return SubMenu;
        }
        #endregion
    }
}
