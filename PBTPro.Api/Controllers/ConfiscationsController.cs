/*
Project: PBT Pro
Description: Confiscations API controller to handle confiscations Form Field
Author: Fakhrul
Date: January 2025
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
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ConfiscationsController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private string LoggerName = "administrator";
        private readonly string _feature = "CONFISCATIONS"; // follow module name (will be used in logging result to user)
        private readonly ILogger<ConfiscationsController> _logger;

        public ConfiscationsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<ConfiscationsController> logger, IHubContext<PushDataHub> hubContext, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_confiscation>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_confiscations.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _tenantDBContext.trn_confiscations.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] trn_confiscation InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                trn_confiscation trn_confiscation = new trn_confiscation
                {
                    owner_name = InputModel.owner_name,
                    owner_icno = InputModel.owner_icno,
                    //owner_telno = InputModel.owner_telno,
                    business_addr = InputModel.business_addr,

                    cfsc_ref_no = InputModel.cfsc_ref_no,

                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                    modifier_id = runUserID,
                    modified_at = DateTime.Now,
                };

                _tenantDBContext.trn_confiscations.Add(trn_confiscation);
                await _tenantDBContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    trn_cfsc_id = trn_confiscation.trn_cfsc_id,

                    owner_name = trn_confiscation.owner_name,
                    owner_icno = trn_confiscation.owner_icno,
                    //owner_telno = trn_confiscation.owner_telno,
                    business_addr = trn_confiscation.business_addr,

                    cfsc_ref_no = InputModel.cfsc_ref_no,

                    is_deleted = trn_confiscation.is_deleted,
                    created_at = trn_confiscation.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta sitaan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] trn_confiscation InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_confiscations.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.business_name))
                {
                    return Error("", SystemMesg(_feature, "BUSINESS_NAME", MessageTypeEnum.Error, string.Format("Ruangan nama perniagaan diperlukan")));
                }

                #endregion

                formField.owner_name = InputModel.owner_name;
                formField.owner_icno = InputModel.owner_icno;
                //formField.owner_telno = InputModel.owner_telno;
                formField.business_addr = InputModel.business_addr;

                formField.cfsc_ref_no = InputModel.cfsc_ref_no;

                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.trn_confiscations.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_confiscations.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_confiscations.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetConfiscationListByUserId(int UserId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var totalCount = await _tenantDBContext.trn_confiscations
                    .Where(n => n.creator_id == UserId)
                    .CountAsync();

                var confiscation_lists = await (from n in _tenantDBContext.trn_confiscations
                                                where n.creator_id == UserId
                                            select new
                                            {
                                                n.cfsc_ref_no,
                                                n.section_act_id,
                                                n.act_type_id,
                                                n.created_at,
                                                n.modified_at,
                                            }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = totalCount,
                    confiscation_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        // can be deleted later.. this is created so that it can be called from other places..
        private bool UnitExists(int id)
        {
            return (_dbContext.ref_genders?.Any(e => e.gen_id == id)).GetValueOrDefault();
        }

    }
}
