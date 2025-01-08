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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityOptions _identityOptions;
        private readonly string _feature = "ROLE";


        public RoleController(PBTProDbContext dbContext, ILogger<RoleController> logger, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> identityOptions) : base(dbContext)
        {
            _logger = logger;
            _roleManager = roleManager;
            _identityOptions = identityOptions.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationRole>>> GetRoles()
        {
            var roles = await _dbContext.Roles
                .Where(r => !r.IsDeleted) // Example: Filter out deleted roles
                .ToListAsync();
            return Ok(roles);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var roles = await _dbContext.Roles.AsNoTracking().ToListAsync();
                return Ok(roles, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ApplicationRole InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                //check asp table role if exist
                if (await _roleManager.RoleExistsAsync(InputModel.Name))
                {
                    return Error("", SystemMesg(_feature, "ROLE_ISEXISTS", MessageTypeEnum.Error, string.Format("Peranan telah wujud")));
                }
                #endregion

                //create at aspdotnet
                if (!string.IsNullOrWhiteSpace(InputModel.Name))
                {
                    await _roleManager.CreateAsync(new IdentityRole(InputModel.Name));
                }

                if (!string.IsNullOrWhiteSpace(InputModel.Name))
                {
                    ApplicationRole ar = new ApplicationRole
                    {
                        Name = InputModel.Name,
                        RoleDesc = InputModel.RoleDesc,
                        IsDefaultRole = InputModel.IsDefaultRole,
                        IsDeleted = false,
                        CreatorId = runUserID,
                        CreatedAt = DateTime.Now,
                    };

                    _dbContext.Roles.Add(ar);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(InputModel, SystemMesg(_feature, "Create", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));

            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ApplicationRole InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == InputModel.Id);
                if (role == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                role.Name = InputModel.Name;
                role.RoleDesc = InputModel.RoleDesc;
                role.ModifierId = runUserID;
                role.ModifiedAt = DateTime.Now;
                role.IsDefaultRole = InputModel.IsDefaultRole;

                _dbContext.Roles.Update(role);
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
                var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);
                if (role == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.Roles.Remove(role);
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
                var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);

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
