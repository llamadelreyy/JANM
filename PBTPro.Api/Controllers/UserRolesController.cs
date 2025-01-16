/*
Project: PBT Pro
Description: user api, to handle user role related action
Author: farhana
Date: january 2024
Version: 1.0
Additional Notes:
- 

Changes Logs:
09/01/2025 - adding crud for assigning user to role
*/
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using static System.Collections.Specialized.BitVector32;
using System.Reactive;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class UserRolesController : IBaseController
    {
        private readonly ILogger<UserRolesController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityOptions _identityOptions;
        private readonly string _feature = "USER_ROLE";

        public UserRolesController(PBTProDbContext dbContext, ILogger<UserRolesController> logger, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> identityOptions) : base(dbContext)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _identityOptions = identityOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var users = await _dbContext.Users.AsNoTracking().ToListAsync();
                var roles = await _dbContext.Roles.AsNoTracking().ToListAsync();
                var userroles = await _dbContext.UserRoles.AsNoTracking().ToListAsync();
                List<UserRoleModel> UserRoleModel = new List<UserRoleModel>();

                UserRoleModel = (from userrole in userroles
                                 join role in roles on userrole.RoleId equals role.Id
                                 join user in users on userrole.UserId equals user.Id
                                 select new UserRoleModel
                                 {
                                     UserId = user.Id,
                                     UserRoleId = userrole.UserId,
                                     FullName = user.full_name,
                                     UserName = user.UserName,
                                     RoleName = role.Name,
                                     RoleDesc = role.RoleDesc,
                                     CreatedAt = userrole.CreatedAt,

                                 }).ToList();

                return Ok(UserRoleModel, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserRoleId == Id);

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

        [HttpPost]
        public async Task<IActionResult> AssignUserRole([FromBody] UserRoleModel InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.UserRoles.FirstOrDefaultAsync();
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var userExists = await _dbContext.Users.Select(x => new { x.Id, x.UserName }).FirstOrDefaultAsync(x => x.Id == InputModel.UserId);
                if (userExists == null)
                {
                    return Error("", "Pengguna tidak wujud.");
                }

                #endregion

                #region store data
                ApplicationUserRole userroles = new ApplicationUserRole
                {
                    RoleId = InputModel.RoleId,
                    UserId = InputModel.UserId,
                    CreatorId = runUserID,
                    CreatedAt = DateTime.Now,
                };

                _dbContext.UserRoles.Add(userroles);
                await _dbContext.SaveChangesAsync();

                #endregion

                return Ok(userroles, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana.")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] UserRoleModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserRoleId == InputModel.UserRoleId);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                formField.RoleId = InputModel.RoleId;
                formField.ModifierId = runUserID;
                formField.ModifiedAt = DateTime.Now;

                _dbContext.UserRoles.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
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
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserRoleId == Id);
                formField.IsDeleted = true;

                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.UserRoles.Update(formField);
                await _dbContext.SaveChangesAsync();

                //_dbContext.UserRoles.Remove(formField);
                //await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool RolesExists(int id)
        {
            return (_dbContext.UserRoles?.Any(e => e.UserRoleId == id)).GetValueOrDefault();
        }
    }
}
