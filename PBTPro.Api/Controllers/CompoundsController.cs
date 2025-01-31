/*
Project: PBT Pro
Description: Compounds API controller to handle compounds Form Field
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
using static DevExpress.Utils.MVVM.Internal.ILReader;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CompoundsController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private string LoggerName = "administrator";
        private readonly string _feature = "COMPOUNDS"; // follow module name (will be used in logging result to user)

        public CompoundsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<CompoundsController> logger, IHubContext<PushDataHub> hubContext, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_compound>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_compounds.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _tenantDBContext.trn_compounds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] trn_compound InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                trn_compound trn_compound = new trn_compound
                {
                    // owner_name = InputModel.owner_name,
                    owner_icno = InputModel.owner_icno,
                    owner_telno = InputModel.owner_telno,
                    business_addr = InputModel.business_addr,

                    cmpd_ref_no = InputModel.cmpd_ref_no,
                    act_type_id = InputModel.act_type_id,
                    section_act_id = InputModel.section_act_id,
                    instruction = InputModel.instruction,
                    offs_location = InputModel.offs_location,
                    amt_cmpd = InputModel.amt_cmpd,
                    deliver_id = InputModel.deliver_id,
                    proof_img1 = InputModel.proof_img1,
                    proof_img2 = InputModel.proof_img2,
                    proof_img3 = InputModel.proof_img3,
                    proof_img4 = InputModel.proof_img4,
                    proof_img5 = InputModel.proof_img5,
                     
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _tenantDBContext.trn_compounds.Add(trn_compound);
                await _tenantDBContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    trn_cmpd_id = InputModel.trn_cmpd_id,

                    // owner_name = trn_compound.owner_name,
                    owner_icno = trn_compound.owner_icno,
                    owner_telno = trn_compound.owner_telno,
                    business_addr = trn_compound.business_addr,

                    cmpd_ref_no = trn_compound.cmpd_ref_no,
                    act_type_id = trn_compound.act_type_id,
                    section_act_id = trn_compound.section_act_id,
                    instruction = trn_compound.instruction,
                    offs_location = trn_compound.offs_location,
                    amt_cmpd = trn_compound.amt_cmpd,
                    deliver_id = trn_compound.deliver_id,
                    proof_img1 = trn_compound.proof_img1,
                    proof_img2 = trn_compound.proof_img2,
                    proof_img3 = trn_compound.proof_img3,
                    proof_img4 = trn_compound.proof_img4,
                    proof_img5 = trn_compound.proof_img5,

                    is_deleted = trn_compound.is_deleted,
                    created_at = trn_compound.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta kompaun")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] trn_compound InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_compounds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.business_name))
                {
                    return Error("", SystemMesg(_feature, "BUSINESS_NAME", MessageTypeEnum.Error, string.Format("Ruangan nama perniagaan diperlukan")));
                }

                #endregion

                // formField.owner_name = InputModel.owner_name;
                formField.owner_icno = InputModel.owner_icno;
                formField.owner_telno = InputModel.owner_telno;
                formField.business_addr = InputModel.business_addr;

                formField.cmpd_ref_no = InputModel.cmpd_ref_no;
                formField.act_type_id = InputModel.act_type_id;
                formField.section_act_id = InputModel.section_act_id;
                formField.instruction = InputModel.instruction;
                formField.offs_location = InputModel.offs_location;
                formField.amt_cmpd = InputModel.amt_cmpd;
                formField.deliver_id = InputModel.deliver_id;
                formField.proof_img1 = InputModel.proof_img1;
                formField.proof_img2 = InputModel.proof_img2;
                formField.proof_img3 = InputModel.proof_img3;
                formField.proof_img4 = InputModel.proof_img4;
                formField.proof_img5 = InputModel.proof_img5;

                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.trn_compounds.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
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
                var formField = await _tenantDBContext.trn_compounds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_compounds.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
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
