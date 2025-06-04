/*
Project: PBT Pro
Description: License Owner controller
Author: Ismail
Date: June 2025
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
    public class LicenseOwnerController : IBaseController
    {
        private readonly ILogger<LicenseOwnerController> _logger;
        private readonly string _feature = "LICENSE_OWNER";

        public LicenseOwnerController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tenantDBContext, ILogger<LicenseOwnerController> logger) : base(dbContext)
        {
            _logger = logger;
            _tenantDBContext = tenantDBContext;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var data = await _tenantDBContext.mst_owner_licensees.Where(x => x.is_deleted != true).OrderBy(x => x.owner_id).AsNoTracking().ToListAsync();

                if (data.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var menu = await _tenantDBContext.mst_owner_licensees.FirstOrDefaultAsync(x => x.owner_id == Id);

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
        public async Task<IActionResult> Add([FromBody] mst_owner_licensee InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.owner_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                mst_owner_licensee owner = new mst_owner_licensee
                {
                    owner_icno = InputModel.owner_icno,
                    owner_name = InputModel.owner_name,
                    owner_email = InputModel.owner_email,
                    owner_addr = InputModel.owner_addr,
                    district_code = InputModel.district_code,
                    state_code = InputModel.state_code,
                    owner_telno = InputModel.owner_telno,
                    town_id = InputModel.town_id,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _tenantDBContext.mst_owner_licensees.Add(owner);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(owner, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah pemilik")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] mst_owner_licensee InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var owner = await _tenantDBContext.mst_owner_licensees.FirstOrDefaultAsync(x => x.owner_id == Id);
                if (owner == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.owner_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                owner.owner_icno = InputModel.owner_icno;
                owner.owner_name = InputModel.owner_name;
                owner.owner_email = InputModel.owner_email;
                owner.owner_addr = InputModel.owner_addr;
                owner.district_code = InputModel.district_code;
                owner.state_code = InputModel.state_code;
                owner.owner_telno = InputModel.owner_telno;
                owner.town_id = InputModel.town_id;
                owner.modifier_id = runUserID;
                owner.modified_at = DateTime.Now;

                _tenantDBContext.mst_owner_licensees.Update(owner);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(owner, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai pemilik")));
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
                var owner = await _tenantDBContext.mst_owner_licensees.FirstOrDefaultAsync(x => x.owner_id == Id);
                if (owner == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _tenantDBContext.mst_owner_licensees.Remove(owner);
                    await _tenantDBContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    owner.is_deleted = true;
                    owner.modifier_id = runUserID;
                    owner.modified_at = DateTime.Now;

                    _tenantDBContext.mst_owner_licensees.Update(owner);
                    await _tenantDBContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(owner, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang pemilik")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        #region Private Logic

        #endregion
    }
}
