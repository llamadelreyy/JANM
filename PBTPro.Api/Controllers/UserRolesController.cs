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
using System.Data;

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
                                 where userrole.IsDeleted == false
                                 select new UserRoleModel
                                 {
                                     UserId = user.Id,
                                     UserRoleId = userrole.UserRoleId,
                                     FullName = user.full_name,
                                     UserName = user.UserName,
                                     RoleName = role.Name,
                                     RoleDesc = role.RoleDesc,
                                     CreatedAt = userrole.CreatedAt,
                                     RoleId = role.Id,

                                 }).ToList();

                return Ok(UserRoleModel, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserRole([FromBody] List<UserRoleModel> InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                foreach (var userRoleModel in InputModel)
                {
                    var userExists = await _dbContext.Users
                        .Where(x => x.Id == userRoleModel.UserId)
                        .FirstOrDefaultAsync();

                    if (userExists == null)
                    {
                        return Error("", $"Pengguna dengan ID {userRoleModel.UserId} tidak wujud.");
                    }

                    if (userRoleModel.Roles == null || userRoleModel.Roles.Count == 0)
                    {
                        return Error("", "Peranan tidak sah.");
                    }

                    // Validate roles
                    HashSet<int> roleIdsSet = new HashSet<int>(userRoleModel.Roles);
                    var validRoleIds = await _dbContext.Roles
                        .Where(r => roleIdsSet.Contains(r.Id) && !r.IsDeleted)
                        .Select(r => r.Id)
                        .ToListAsync();

                    var invalidRoleIds = roleIdsSet.Except(validRoleIds).ToList();
                    if (invalidRoleIds.Any())
                    {
                        return Error("", "Peranan tidak sah.");
                    }

                    var userRoleIds = await _dbContext.UserRoles
                        .Where(x => x.UserId == userExists.Id)
                        .ToListAsync();

                    var newRoles = roleIdsSet
                        .Except(userRoleIds.Select(r => r.RoleId))
                        .ToList();

                    var delRoles = userRoleIds
                        .Where(x => !roleIdsSet.Contains(x.RoleId))
                        .ToList();

                    // If roles need to be updated
                    if (newRoles.Any() || delRoles.Any())
                    {
                        foreach (var newRoleId in newRoles)
                        {
                            var newRole = await _dbContext.Roles
                                .FirstOrDefaultAsync(r => r.Id == newRoleId);

                            if (newRole != null && !await _roleManager.RoleExistsAsync(newRole.Name))
                            {
                                var applicationRole = new ApplicationRole
                                {
                                    Name = newRole.Name
                                };
                                await _roleManager.CreateAsync(applicationRole);
                            }

                            var newUcUserRole = new ApplicationUserRole
                            {
                                UserId = userExists.Id,
                                RoleId = newRoleId,
                                CreatedAt = DateTime.Now,
                                CreatorId = runUserID
                            };

                            _dbContext.UserRoles.Add(newUcUserRole);
                            await _userManager.AddToRoleAsync(userExists, newRole.Name);
                        }

                        foreach (var delRole in delRoles)
                        {
                            _dbContext.UserRoles.Remove(delRole);
                            var role = await _dbContext.Roles
                                .FirstOrDefaultAsync(r => r.Id == delRole.RoleId);
                            if (role != null)
                            {
                                await _userManager.RemoveFromRoleAsync(userExists, role.Name);
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                }

                return Ok(new { message = "Roles successfully assigned/removed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", "An unexpected error occurred. Please try again later.");
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

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool RolesExists(int id)
        {
            return (_dbContext.UserRoles?.Any(e => e.UserRoleId == id)).GetValueOrDefault();
        }
    }
}
