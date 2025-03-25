/*
Project: PBT Pro
Description: Inspections API controller to handle inspections Form Field
Author: Fakhrul
Date: January 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
27/02/2025 - revamp table & logic
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class InspectionsController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly string _feature = "INSPECTIONS"; // follow module name (will be used in logging result to user)
        private readonly ILogger<InspectionsController> _logger;
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };

        public InspectionsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<InspectionsController> logger, PBTProTenantDbContext tntdbContext, IEmailSender emailSender) : base(dbContext)
        {
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_inspect>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_inspects.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var inspect = await _tenantDBContext.trn_inspects.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);

                if (inspect == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var result = MapEntity<patrol_inspect_view_model>(inspect);
                result.proofs = await _tenantDBContext.trn_inspect_imgs.Where(x => x.trn_inspect_id == inspect.trn_inspect_id).AsNoTracking().ToListAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] patrol_inspect_input_model InputModel)
        {
            try
            {
                if (InputModel.witnesses == null || InputModel.witnesses.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["witnesses"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["witnesses"].ToString();
                        var fixedJson = "[" + rawItemReq + "]";
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_inspect_witness>>(fixedJson);
                    }
                }

                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.inspect_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Nota Pemeriksaan diperlukan")));
                }

                if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                {
                    foreach (var ip in InputModel.proofs)
                    {
                        if (!IsFileExtensionAllowed(ip, _imageFileExt))
                        {
                            var imageFileExtString = String.Join(", ", _imageFileExt.ToList());
                            List<string> param = new List<string> { imageFileExtString };
                            return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong ([0])."), param));
                        }

                        if (!IsFileSizeWithinLimit(ip, _maxImageFileSize))
                        {
                            List<string> param = new List<string> { FormatFileSize(_maxImageFileSize) };
                            return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                        }
                    }
                }
                #endregion

                #region Start Transaction
                using (var transaction = await _tenantDBContext.Database.BeginTransactionAsync())
                {
                    trn_inspect inspect;
                    try
                    {
                        #region Main Data
                        inspect = new trn_inspect
                        {
                            owner_icno = InputModel.owner_icno,
                            inspect_ref_no = InputModel.inspect_ref_no,
                            notes = InputModel.notes,
                            offs_location = InputModel.offs_location,
                            inspect_longitude = InputModel.inspect_longitude,
                            inspect_latitude = InputModel.inspect_latitude,
                            trnstatus_id = InputModel.trnstatus_id,
                            license_id = InputModel.license_id,
                            schedule_id = InputModel.schedule_id,
                            tax_accno = InputModel.tax_accno,
                            is_tax = InputModel.is_tax,
                            user_id = InputModel.user_id,
                            dept_id = InputModel.dept_id,
                            is_deleted = false,
                            creator_id = runUserID,
                            created_at = DateTime.Now,
                        };

                        _tenantDBContext.trn_inspects.Add(inspect);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var witnesses = new List<trn_witness>();
                        if (InputModel.witnesses != null && InputModel.witnesses.Count > 0)
                        {
                            foreach (var w in InputModel.witnesses)
                            {
                                var witness = new trn_witness();
                                witness.trn_id = inspect.trn_inspect_id;
                                witness.trn_type = "INSPECTION";
                                witness.name = w.name;
                                witness.user_id = w.user_id;
                                witness.is_deleted = false;
                                witness.creator_id = runUserID;
                                witness.created_at = DateTime.Now;
                                witnesses.Add(witness);
                            }

                            if (witnesses.Count > 0)
                            {
                                _tenantDBContext.trn_witnesses.AddRange(witnesses);
                            }
                        }
                        #endregion

                        #region Proof Image
                        var proofs = new List<trn_inspect_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(inspect.inspect_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(inspect);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(inspect);
                                var proof = new trn_inspect_img();
                                proof.trn_inspect_id = inspect.trn_inspect_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_inspect_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                    }

                    if(InputModel.dept_id.HasValue && InputModel.dept_id > 0)
                    {
                        var deptInfo = await _tenantDBContext.ref_departments.AsNoTracking().FirstOrDefaultAsync(x=>x.dept_id == InputModel.dept_id);
                        if(deptInfo != null && !string.IsNullOrWhiteSpace(deptInfo?.dept_email))
                        {
                            await SendInspectionFollowUpNotice(deptInfo.dept_email, deptInfo.dept_name, inspect);
                        }
                    }
                    return Ok(inspect, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta nota pemeriksaan")));
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromForm] patrol_inspect_input_model InputModel)
        {
            try
            {
                if (InputModel.witnesses == null || InputModel.witnesses.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["witnesses"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["witnesses"].ToString();
                        var fixedJson = "[" + rawItemReq + "]";
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_inspect_witness>>(fixedJson);
                    }
                }

                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.inspect_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Nota Pemeriksaan diperlukan")));
                }

                var inspect = await _tenantDBContext.trn_inspects.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
                if (inspect == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                {
                    foreach (var ip in InputModel.proofs)
                    {
                        if (!IsFileExtensionAllowed(ip, _imageFileExt))
                        {
                            var imageFileExtString = String.Join(", ", _imageFileExt.ToList());
                            List<string> param = new List<string> { imageFileExtString };
                            return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong ([0])."), param));
                        }

                        if (!IsFileSizeWithinLimit(ip, _maxImageFileSize))
                        {
                            List<string> param = new List<string> { FormatFileSize(_maxImageFileSize) };
                            return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                        }
                    }
                }
                #endregion

                #region Start Transaction
                using (var transaction = await _tenantDBContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        #region Main Data
                        inspect.owner_icno = InputModel.owner_icno;
                        inspect.inspect_ref_no = InputModel.inspect_ref_no;
                        inspect.notes = InputModel.notes;
                        inspect.offs_location = InputModel.offs_location;
                        inspect.inspect_longitude = InputModel.inspect_longitude;
                        inspect.inspect_latitude = InputModel.inspect_latitude;
                        inspect.trnstatus_id = InputModel.trnstatus_id;
                        inspect.license_id = InputModel.license_id;
                        inspect.schedule_id = InputModel.schedule_id;
                        inspect.tax_accno = InputModel.tax_accno;
                        inspect.is_tax = InputModel.is_tax;
                        inspect.user_id = InputModel.user_id;
                        inspect.is_deleted = false;
                        inspect.modifier_id = runUserID;
                        inspect.modified_at = DateTime.Now;

                        _tenantDBContext.trn_inspects.Add(inspect);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var existingWitness = await _tenantDBContext.trn_witnesses.Where(x => x.trn_type == "INSPECTION" && x.trn_id == inspect.trn_inspect_id).ToListAsync();
                        if (existingWitness != null)
                        {
                            _tenantDBContext.trn_witnesses.RemoveRange(existingWitness);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var witnesses = new List<trn_witness>();
                        if (InputModel.witnesses != null && InputModel.witnesses.Count > 0)
                        {
                            foreach (var w in InputModel.witnesses)
                            {
                                var witness = new trn_witness();
                                witness.trn_id = inspect.trn_inspect_id;
                                witness.trn_type = "INSPECTION";
                                witness.name = w.name;
                                witness.user_id = w.user_id;
                                witness.is_deleted = false;
                                witness.creator_id = runUserID;
                                witness.created_at = DateTime.Now;
                                witnesses.Add(witness);
                            }

                            if (witnesses.Count > 0)
                            {
                                _tenantDBContext.trn_witnesses.AddRange(witnesses);
                            }
                        }
                        #endregion

                        #region Proof Image
                        var existingProofs = await _tenantDBContext.trn_inspect_imgs.Where(x => x.trn_inspect_id == inspect.trn_inspect_id).ToListAsync();
                        if (existingProofs != null)
                        {
                            var UploadPath = await getUploadPath(inspect);
                            foreach (var existingProof in existingProofs)
                            {
                                var Fullpath = Path.Combine(UploadPath, existingProof.filename);
                                await rmvExistFile(Fullpath);
                            }
                            _tenantDBContext.trn_inspect_imgs.RemoveRange(existingProofs);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var proofs = new List<trn_inspect_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(inspect.inspect_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(inspect);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(inspect);
                                var proof = new trn_inspect_img();
                                proof.trn_inspect_id = inspect.trn_inspect_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_inspect_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        await transaction.CommitAsync();
                        return Ok(inspect, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai nota pemeriksaan")));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                    }
                }
                #endregion
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
                var formField = await _tenantDBContext.trn_inspects.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_inspects.Remove(formField); 
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetInspectionListByUserId(int UserId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var inspection_lists = await (from n in _tenantDBContext.trn_inspects
                                              where n.creator_id == UserId
                                          select new
                                          {
                                              n.inspect_ref_no,
                                              n.created_at,
                                              n.modified_at,
                                              n.trnstatus_id,
                                          }).ToListAsync();

                // Check if no record was found
                if (inspection_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = inspection_lists.Count,
                    inspection_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{ScheduleId}")]
        public async Task<IActionResult> GetInspectionListBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var inspection_lists = await (from n in _tenantDBContext.trn_inspects
                                              where n.schedule_id == ScheduleId
                                              select new
                                              {
                                                  n.inspect_ref_no,
                                                  n.created_at,
                                                  n.modified_at,
                                                  n.trnstatus_id,
                                              }).ToListAsync();

                // Check if no record was found
                if (inspection_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = inspection_lists.Count,
                    inspection_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{TaxAccNo}")]
        public async Task<IActionResult> GetInspectionListByTaxAccNo(string TaxAccNo)
        {
            try
            {
                var resultData = new List<dynamic>();

                var inspection_lists = await (from n in _tenantDBContext.trn_inspects
                                              where n.tax_accno == TaxAccNo
                                              select new
                                              {
                                                  n.inspect_ref_no,
                                                  n.created_at,
                                                  n.modified_at,
                                                  n.trnstatus_id,
                                              }).ToListAsync();

                // Check if no record was found
                if (inspection_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = inspection_lists.Count,
                    inspection_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{LicenseAccNo}")]
        public async Task<IActionResult> GetInspectionListByLicenseAccNo(string LicenseAccNo)
        {
            try
            {
                var resultData = new List<dynamic>();
                var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.license_accno == LicenseAccNo);
                var inspection_lists = await (from n in _tenantDBContext.trn_inspects
                                              where n.license_id == licenseInfo.licensee_id
                                              select new
                                              {
                                                  n.inspect_ref_no,
                                                  n.created_at,
                                                  n.modified_at,
                                                  n.trnstatus_id,
                                              }).ToListAsync();

                // Check if no record was found
                if (inspection_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = inspection_lists.Count,
                    inspection_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{LicenseId}")]
        public async Task<IActionResult> GetInspectionListByLicenseId(int LicenseId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var inspection_lists = await (from n in _tenantDBContext.trn_inspects
                                              where n.schedule_id == LicenseId
                                              select new
                                              {
                                                  n.inspect_ref_no,
                                                  n.created_at,
                                                  n.modified_at,
                                                  n.trnstatus_id,
                                              }).ToListAsync();

                // Check if no record was found
                if (inspection_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = inspection_lists.Count,
                    inspection_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Testing API
        // For Testing Purpose ONLY WIll be removed after finalization
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SentFollowUpNotice(int InspectionId)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                var inspect = await _tenantDBContext.trn_inspects.AsNoTracking().FirstOrDefaultAsync(x => x.trn_inspect_id == InspectionId);

                var deptInfo = await _tenantDBContext.ref_departments.AsNoTracking().FirstOrDefaultAsync(x => x.dept_id == inspect.dept_id);
                if (deptInfo != null && !string.IsNullOrWhiteSpace(deptInfo?.dept_email))
                {
                    await SendInspectionFollowUpNotice(deptInfo.dept_email, deptInfo.dept_name, inspect);
                }

                return Ok(inspect, SystemMesg(_feature, "TEST_NOTICE", MessageTypeEnum.Success, string.Format("Berjaya menghantar notis")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_inspect_view>>> ListReport()
        {
            try
            {
                var tenantInspects = await _tenantDBContext.trn_inspects
                   .AsNoTracking()
                   .ToListAsync();

                var tenantBusiness = (await _tenantDBContext.mst_owner_licensees
                    .Where(mol => !string.IsNullOrEmpty(mol.owner_icno))
                    .Select(mol => new { mol.owner_icno, mol.owner_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(mol => mol.owner_icno, mol => mol);

                var tenantPremis = (await _tenantDBContext.mst_owner_premis
                   .Where(mop => !string.IsNullOrEmpty(mop.owner_icno))
                   .Select(mop => new { mop.owner_icno, mop.owner_name })
                   .AsNoTracking()
                   .ToListAsync())
                   .ToDictionary(mop => mop.owner_icno, mop => mop);

                var tenantLicensees = (await _tenantDBContext.mst_licensees
                    .Where(ml => !string.IsNullOrEmpty(ml.license_accno))
                    .Select(ml => new { ml.licensee_id, ml.license_accno, ml.business_name, ml.ssm_no, ml.business_addr })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(ml => ml.licensee_id, ml => ml);

                var tenantStatuses = await _tenantDBContext.ref_trn_statuses
                    .Select(rts => new { rts.status_id, rts.status_name })
                    .AsNoTracking()
                    .ToListAsync();

                var tenantPatrolSchedules = (await _tenantDBContext.mst_patrol_schedules
                     .Select(mps => new { mps.schedule_id, mps.idno })
                     .AsNoTracking()
                     .ToListAsync())
                     .ToDictionary(mps => mps.schedule_id, mps => mps.idno);

                var tenantWitness = (await _tenantDBContext.trn_witnesses
                    .Select(tw => new { tw.trn_id, tw.name })
                    .AsNoTracking()
                    .ToListAsync())
                    .GroupBy(tw => tw.trn_id)
                    .ToDictionary(g => g.Key, g => g.Select(tw => tw.name).ToList());

                var users = (await _dbContext.Users
                    .Select(u => new { u.IdNo, u.full_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(u => u.IdNo, u => u.full_name);

                var tenantInspectImgs = (await _tenantDBContext.trn_inspect_imgs
                    .Select(img => new { img.trn_inspect_id, img.pathurl })
                    .AsNoTracking()
                    .ToListAsync())
                    .GroupBy(img => img.trn_inspect_id)
                    .ToDictionary(g => g.Key, g => g.Select(img => img.pathurl).Distinct().ToList());

                var tenantDepartments = (await _tenantDBContext.ref_departments
                   .Select(dpt => new { dpt.dept_id, dpt.dept_code, dpt.dept_name })
                   .AsNoTracking()
                   .ToListAsync())
                   .ToDictionary(dpt => dpt.dept_id, dpt => dpt.dept_name);


                var results = tenantInspects.Select(tI =>
                {
                    tenantBusiness.TryGetValue(tI.owner_icno, out var ownerlicense);
                    tenantPremis.TryGetValue(tI.owner_icno, out var ownerPremis);
                    //tenantLicensees.TryGetValue((int)tI?.license_id, out var licensee);
                    tenantDepartments.TryGetValue((int)tI?.dept_id, out var department);
                    tenantPatrolSchedules.TryGetValue((int)tI.schedule_id, out var officerId);
                    users.TryGetValue(officerId, out var officer);
                    tenantInspectImgs.TryGetValue(tI.trn_inspect_id, out var images);
                    tenantWitness.TryGetValue(tI.trn_inspect_id, out var witnesses);

                    var licensee = tI?.license_id.HasValue == true
                    ? tenantLicensees.GetValueOrDefault(tI.license_id.Value)
                    : null;

                    return new trn_inspect_view
                    {
                        id_nota = tI.trn_inspect_id,
                        no_lesen = licensee?.license_accno,
                        nama_perniagaan = licensee?.business_name,
                        nama_pemilik = ownerlicense?.owner_name,
                        no_rujukan = tI.inspect_ref_no,
                        arahan = tI.notes,
                        lokasi_kesalahan = tI.offs_location,
                        TarikhData = tI.created_at,
                        nama_pegawai = officer,
                        ssm_no = licensee?.ssm_no,
                        alamat_perniagaan = licensee?.business_addr,
                        nama_dokumen = tI.doc_name,
                        pautan_dokumen = tI.doc_pathurl,
                        imej_nota = images ?? new List<string>(),
                        nama_saksi = witnesses != null && witnesses.Any() ? string.Join(", ", witnesses) : "",
                        no_cukai = tI?.tax_accno ?? null,
                        id_jabatan = tI?.dept_id ?? 0,
                        nama_jabatan = department,
                        status_nota_id = (int)tI.trnstatus_id,
                        status_nota = tenantStatuses.FirstOrDefault(s => s.status_id == tI.trnstatus_id)?.status_name,
                        lesen_id = tI?.license_id ?? null,
                        inspect_latitude = tI.inspect_latitude,
                        inspect_longitude = tI.inspect_longitude,
                    };

                }).ToList();

                return Ok(results, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateReport(int Id, [FromBody] trn_inspect_view InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_inspects.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                
                #endregion

                formField.trnstatus_id = InputModel.status_nota_id;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.trn_inspects.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteReport(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();
                int runUserID = await getDefRunUserId();

                #region Validation
                var formField = await _tenantDBContext.trn_inspects.FirstOrDefaultAsync(x => x.trn_inspect_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _tenantDBContext.trn_inspects.Remove(formField);
                    await _tenantDBContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    formField.is_deleted = true;
                    formField.modifier_id = runUserID;
                    formField.modified_at = DateTime.Now;

                    _tenantDBContext.trn_inspects.Update(formField);
                    await _tenantDBContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Private Logic
        private async Task<string?> getUploadPath(trn_inspect? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");

            result = await getBaseUploadPath("inspect", stringDate);

            return result;
        }

        private async Task<string?> getViewUrl(trn_inspect? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");
            result = await getBaseViewUrl("inspect", stringDate);
            return result;
        }

        private async Task<string?> getBaseUploadPath(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "BaseUploadPath").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                string? tenantId = _tenantId.ToString();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    result = Path.Combine(result, tenantId);
                }

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = Path.Combine(result, lv1);
                    if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = Path.Combine(result, lv2);
                        if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = Path.Combine(result, lv3);
                            if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = Path.Combine(result, lv4);
                                if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private async Task<string?> getBaseViewUrl(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "ImageViewUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                string? tenantId = _tenantId.ToString();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    result = result + "/" + tenantId;
                }

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = result + "/" + lv1;
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = result + "/" + lv2;
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = result + "/" + lv3;
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = result + "/" + lv4;
                            }
                        }
                    }
                }
            }
            return result;
        }


        private async Task<bool> SendInspectionFollowUpNotice(string recipient, string dept_name, trn_inspect inspectInfo)
        {
            try
            {
                //Default Email Template
                EmailContent defaultContent = new EmailContent
                {
                    subject = "Permohonan Pemeriksaan Premis",
                    body = "Salam Sejahtera,<br/><br/>" +
                    "Saya berharap ini mendapati tuan / puan dalam keadaan sihat dan ceria.Saya,"+
                    "[0],"+
                    "anggota penguatkuasaan dari [1]," +
                    "ingin memohon bantuan pihak tuan / puan untuk melakukan pemeriksaan ke atas premis berikut:<br/><br/>" +
                    "Nama Premis: [2]<br/><br/>" +
                    "Alamat Premis: [3]<br/><br/>" +
                    "Tujuan Pemeriksaan: [4]<br/><br/>" +
                    "Saya telah melampirkan dokumen sokongan yang diperlukan untuk rujukan pihak tuan / puan.Saya "+
                    "berharap pihak tuan / puan dapat mempertimbangkan permohonan ini dan menjadualkan pemeriksaan "+
                    "secepat mungkin.<br/><br/>" +
                    "Terima kasih atas perhatian dan kerjasama tuan / puan.Saya boleh dihubungi melalui [5] atau [6] "+
                    "jika terdapat sebarang pertanyaan.<br/><br/>" +
                    "Sekian, terima kasih.<br/><br/>" +
                    "[0]<br/>" +
                    "Anggota Penguatkuasaan<br/>" +
                    "[1]",
                };

                var reporterInfo = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == inspectInfo.creator_id);
                var reporterUnit = await _tenantDBContext.ref_units.AsNoTracking().FirstOrDefaultAsync(x => x.unit_id == reporterInfo.unit_id);
                var premisInfo = new { name = "", address = "" };

                if (inspectInfo.license_id.HasValue && inspectInfo.license_id.Value > 0)
                {
                    var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.licensee_id == inspectInfo.license_id);
                    if (licenseInfo != null)
                    {
                        premisInfo = new
                        {
                            name = licenseInfo.business_name,
                            address = licenseInfo.business_addr
                        };
                    }
                } else if (!inspectInfo.license_id.HasValue && !string.IsNullOrWhiteSpace(inspectInfo.tax_accno))
                {
                    var taxInfo = await _tenantDBContext.mst_taxholders.AsNoTracking().FirstOrDefaultAsync(x => x.tax_accno == inspectInfo.tax_accno);
                    if (taxInfo != null)
                    {
                        premisInfo = new
                        {
                            name = "",
                            address = taxInfo.alamat
                        };
                    }
                }

                string[] param = {
                    reporterInfo?.full_name ?? "",//namaPegawai, 
                    reporterUnit?.unit_name ?? "",//Nama Unit Penguatkuasaan, 
                    premisInfo?.name ?? "",//Nama Premis,
                    premisInfo?.address ?? "",//Alamat Premis,
                    inspectInfo?.notes ?? "",//Tujuan Pemeriksaan, 
                    reporterInfo?.PhoneNumber ?? "",//Nombor Telefon, 
                    reporterInfo?.Email ?? ""//Alamat Email 
                };

                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("PATROL_INSPECT_FOLLOWUP_NOTICE", param, defaultContent);

                var emailRs = await emailHelper.QueueEmail(emailContent.subject, emailContent.body, recipient);
                var sentRs = await emailHelper.ForceProcessQueue(emailRs);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return false;
            }
        }
        #endregion
    }
}
