/*
Project: PBT Pro
Description: Inspections API controller to handle inspections Form Field
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
    public class InspectionsController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private string LoggerName = "administrator";
        private readonly string _feature = "INSPECTIONS"; // follow module name (will be used in logging result to user)

        public InspectionsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<InspectionsController> logger, IHubContext<PushDataHub> hubContext, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_inspection>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_inspections.AsNoTracking().ToListAsync();
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
                var parFormfield = await _tenantDBContext.trn_inspections.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);

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
        public async Task<IActionResult> Add([FromBody] trn_inspection InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                trn_inspection trn_inspection = new trn_inspection
                {
                    // owner_name = InputModel.owner_name,
                    owner_icno = InputModel.owner_icno,
                    owner_telno = InputModel.owner_telno,
                    business_addr = InputModel.business_addr,

                    inspect_ref_no = InputModel.inspect_ref_no,
                    dept_id = InputModel.dept_id,
                    notes = InputModel.notes,
                    proof_img1 = InputModel.proof_img1,
                    proof_img2 = InputModel.proof_img2,
                    proof_img3 = InputModel.proof_img3,
                    proof_img4 = InputModel.proof_img4,
                    proof_img5 = InputModel.proof_img5,

                    // not in the Mobile UI
                    business_name = InputModel.business_name,
                    offs_location = InputModel.offs_location, // lokasi kesalahan
                    ntc_latitude = InputModel.ntc_latitude, // lat lokasi kesalahan
                    ntc_longitude = InputModel.ntc_longitude, // lng lokasi kesalahan
                    idno = InputModel.idno, // id pegawai yg keluarkn ticket

                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _tenantDBContext.trn_inspections.Add(trn_inspection);
                await _tenantDBContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    trn_inspect_id = trn_inspection.trn_inspect_id,

                    // owner_name = InputModel.owner_name,
                    owner_icno = trn_inspection.owner_icno,
                    owner_telno = trn_inspection.owner_telno,
                    business_addr = trn_inspection.business_addr,

                    inspect_ref_no = trn_inspection.inspect_ref_no,
                    dept_id = trn_inspection.dept_id,
                    notes = trn_inspection.notes,
                    proof_img1 = trn_inspection.proof_img1,
                    proof_img2 = trn_inspection.proof_img2,
                    proof_img3 = trn_inspection.proof_img3,
                    proof_img4 = trn_inspection.proof_img4,
                    proof_img5 = trn_inspection.proof_img5,

                    // not in the Mobile UI
                    business_name = trn_inspection.business_name,
                    offs_location = trn_inspection.offs_location, // lokasi kesalahan
                    ntc_latitude = trn_inspection.ntc_latitude, // lat lokasi kesalahan
                    ntc_longitude = trn_inspection.ntc_longitude, // lng lokasi kesalahan
                    idno = trn_inspection.idno, // id pegawai yg keluarkn ticket

                    is_deleted = trn_inspection.is_deleted,
                    created_at = trn_inspection.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta nota pemeriksaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] trn_inspection InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_inspections.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
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

                formField.inspect_ref_no = InputModel.inspect_ref_no;
                formField.dept_id = InputModel.dept_id;
                formField.notes = InputModel.notes;
                formField.proof_img1 = InputModel.proof_img1;
                formField.proof_img2 = InputModel.proof_img2;
                formField.proof_img3 = InputModel.proof_img3;
                formField.proof_img4 = InputModel.proof_img4;
                formField.proof_img5 = InputModel.proof_img5;

                // not in the Mobile UI
                formField.business_name = InputModel.business_name;
                formField.offs_location = InputModel.offs_location; // lokasi kesalahan
                formField.ntc_latitude = InputModel.ntc_latitude; // lat lokasi kesalahan
                formField.ntc_longitude = InputModel.ntc_longitude; // lng lokasi kesalahan
                formField.idno = InputModel.idno; // id pegawai yg keluarkn ticket

                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.trn_inspections.Update(formField);
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
                var formField = await _tenantDBContext.trn_inspections.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_inspections.Remove(formField); 
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
