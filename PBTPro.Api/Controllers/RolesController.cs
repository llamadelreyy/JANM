/*
Project: PBT Pro
Description: user api, to handle user related action
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- 

Changes Logs:
15/11/2024 - initial create
18/11/2024 - add field & logic for signature
20/11/2024 - add field & logic for profile avatar
03/12/2024 - change hardcoded upload path & url to refer param table 
*/
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class RolesController : IBaseController
    {
        private readonly ILogger<RolesController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IdentityOptions _identityOptions;
        private readonly string _feature = "ROLE";

        public RolesController(PBTProDbContext dbContext, ILogger<RolesController> logger, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> identityOptions) : base(dbContext)
        {
            _logger = logger;
            _roleManager = roleManager;
            _identityOptions = identityOptions.Value;
        }

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

        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);

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
        public async Task<IActionResult> Add([FromBody] ApplicationRole InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.Roles.FirstOrDefaultAsync();
                
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                if (formField.Name == InputModel.Name)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Peranan telah wujud.")));
                }
                #endregion

                #region store data
                ApplicationRole roles = new ApplicationRole
                {
                    Name = InputModel.Name,
                    RoleDesc = InputModel.RoleDesc,
                    CreatorId = runUserID,
                    CreatedAt = DateTime.Now,
                    ModifierId = runUserID,
                    ModifiedAt = DateTime.Now,
                    IsDeleted = false,
                    //IsTenant = false,
                    IsDefaultRole = false,
                };

                _dbContext.Roles.Add(roles);
                await _dbContext.SaveChangesAsync();

                #endregion

                return Ok(roles, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana.")));
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
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                
                #endregion

                formField.Name = InputModel.Name;
                formField.RoleDesc = InputModel.RoleDesc;
                formField.ModifierId = runUserID;
                formField.ModifiedAt = DateTime.Now;

                _dbContext.Roles.Update(formField);
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
                var formField = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.Roles.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool RolesExists(int id)
        {
            return (_dbContext.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
