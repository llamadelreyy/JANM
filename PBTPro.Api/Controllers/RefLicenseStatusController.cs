/*
Project: PBT Pro
Description: Ref License Status controller
Author: Ismail
Date: June 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
04/01/2025 - initial create
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
    public class RefLicenseStatusController : IBaseController
    {
        private readonly ILogger<RefLicenseStatusController> _logger;
        private readonly string _feature = "REF_LICENSE_STATUS";

        public RefLicenseStatusController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tenantDBContext, ILogger<RefLicenseStatusController> logger) : base(dbContext)
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
                var data = await _tenantDBContext.ref_license_statuses.Where(x => x.is_deleted != true).OrderBy(x => x.status_id).AsNoTracking().ToListAsync();

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
                var menu = await _tenantDBContext.ref_license_statuses.FirstOrDefaultAsync(x => x.status_id == Id);

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
        public async Task<IActionResult> Add([FromBody] ref_license_status InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.status_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                ref_license_status status = new ref_license_status
                {
                    status_name = InputModel.status_name,
                    priority = InputModel.priority,
                    color = InputModel.color,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _tenantDBContext.ref_license_statuses.Add(status);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(status, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah status lesen")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_license_status InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var status = await _tenantDBContext.ref_license_statuses.FirstOrDefaultAsync(x => x.status_id == Id);
                if (status == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.status_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }
                #endregion

                status.status_name = InputModel.status_name;
                status.priority = InputModel.priority;
                status.color = InputModel.color;
                status.modifier_id = runUserID;
                status.modified_at = DateTime.Now;

                _tenantDBContext.ref_license_statuses.Update(status);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(status, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai status lesen")));
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
                var status = await _tenantDBContext.ref_license_statuses.FirstOrDefaultAsync(x => x.status_id == Id);
                if (status == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _tenantDBContext.ref_license_statuses.Remove(status);
                    await _tenantDBContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    status.is_deleted = true;
                    status.modifier_id = runUserID;
                    status.modified_at = DateTime.Now;

                    _tenantDBContext.ref_license_statuses.Update(status);
                    await _tenantDBContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(status, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang status lesen")));
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
