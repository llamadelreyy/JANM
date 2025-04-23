/*
Project: PBT Pro
Description: Ref Relationship API controller to handle receipient relationship Form Field
Author: Ismail
Date: April 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RefRelationshipController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RefPatrolStatusController> _logger;

        private readonly string _feature = "REF_RELATIONSHIP";

        public RefRelationshipController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<RefPatrolStatusController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var data = await _tenantDBContext.ref_relationships.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var parFormfield = await _tenantDBContext.ref_relationships.FirstOrDefaultAsync(x => x.relation_id == Id);

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
        public async Task<IActionResult> Add([FromBody] ref_relationship InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.relation_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan nama hubungan diperlukan")));
                }
                #endregion

                #region store data
                ref_relationship ref_relationship = new ref_relationship
                {
                    relation_name = InputModel.relation_name,
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _tenantDBContext.ref_relationships.Add(ref_relationship);
                await _tenantDBContext.SaveChangesAsync();
                #endregion

                return Ok(ref_relationship, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya tambah data.")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_relationship InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.ref_relationships.FirstOrDefaultAsync(x => x.relation_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.relation_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan nama hubungan diperlukan")));
                }
                #endregion

                formField.relation_name = InputModel.relation_name;
                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.ref_relationships.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai data")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
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
                var formField = await _tenantDBContext.ref_relationships.FirstOrDefaultAsync(x => x.relation_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.ref_relationships.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}