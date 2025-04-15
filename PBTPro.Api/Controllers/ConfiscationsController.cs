/*
Project: PBT Pro
Description: Confiscations API controller to handle confiscations Form Field
Author: Fakhrul
Date: January 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
27/02/2025 - revamp table & logic
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using QuestPDF.Fluent;
using QuestPDF.Helpers;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ConfiscationsController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly string _feature = "CONFISCATIONS"; // follow module name (will be used in logging result to user)
        private readonly ILogger<ConfiscationsController> _logger;
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };

        public ConfiscationsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<ConfiscationsController> logger, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_cfsc>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_cfscs.AsNoTracking().ToListAsync();
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
                var confiscation = await _tenantDBContext.trn_cfscs.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);

                if (confiscation == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var result = MapEntity<patrol_cfsc_view_model>(confiscation);
                result.items = await _tenantDBContext.trn_cfsc_items.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).AsNoTracking().ToListAsync();
                result.proofs = await _tenantDBContext.trn_cfsc_imgs.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).AsNoTracking().ToListAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] patrol_cfsc_input_model InputModel)
        {
            try
            {
                if (InputModel.witnesses == null || InputModel.witnesses.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["witnesses"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["witnesses"].ToString();
                        var fixedJson = rawItemReq;
                        if (!rawItemReq.StartsWith("[") || !rawItemReq.EndsWith("]"))
                        {
                            fixedJson = "[" + rawItemReq + "]";
                        }
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_cfsc_witness>>(fixedJson);
                    }
                }

                if (InputModel.items == null || InputModel.items.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["items"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["items"].ToString();
                        var fixedJson = rawItemReq;
                        if (!rawItemReq.StartsWith("[") || !rawItemReq.EndsWith("]"))
                        {
                            fixedJson = "[" + rawItemReq + "]";
                        }
                        InputModel.items = JsonConvert.DeserializeObject<List<patrol_cfsc_item_model>>(fixedJson);
                    }
                }

                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.cfsc_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Sitaan diperlukan")));
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
                        trn_cfsc confiscation = new trn_cfsc
                        {
                            owner_icno = InputModel.owner_icno,
                            cfsc_ref_no = InputModel.cfsc_ref_no,
                            instruction = InputModel.instruction,
                            offs_location = InputModel.offs_location,
                            scen_id = InputModel.scen_id,
                            cfsc_longitude = InputModel.cfsc_longitude,
                            cfsc_latitude = InputModel.cfsc_latitude,
                            trnstatus_id = InputModel.trnstatus_id,
                            license_id = InputModel.license_id,
                            offense_code = InputModel.offense_code,
                            uuk_code = InputModel.uuk_code,
                            act_code = InputModel.act_code,
                            section_code = InputModel.section_code,
                            schedule_id = InputModel.schedule_id,
                            tax_accno = InputModel.tax_accno,
                            is_tax = InputModel.is_tax,
                            user_id = InputModel.user_id,
                            is_deleted = false,
                            creator_id = runUserID,
                            created_at = DateTime.Now,
                            //2025-04-08 - added new field
                            recipient_name = InputModel.recipient_name,
                            recipient_icno = InputModel.recipient_icno,
                            recipient_telno = InputModel.recipient_telno,
                            recipient_addr = InputModel.recipient_addr,
                            //2025-04-11 - add field for relationship
                            recipient_relation_id = InputModel.recipient_relation_id,
                        };

                        #region receipient signature
                        //2025-04-08 - added new field
                        if (InputModel.recipient_sign != null)
                        {
                            string ImageUploadExt = Path.GetExtension(InputModel.recipient_sign.FileName).ToString().ToLower();
                            string Filename = $"{GetValidFilename(confiscation.cfsc_ref_no)}_receipient_signature{ImageUploadExt}";
                            var UploadPath = await getUploadPath(confiscation);
                            var Fullpath = Path.Combine(UploadPath, Filename);
                            using (var stream = new FileStream(Fullpath, FileMode.Create))
                            {
                                await InputModel.recipient_sign.CopyToAsync(stream);
                            }
                            string pathurl = await getViewUrl(confiscation);
                            confiscation.recipient_sign = $"{pathurl}/{Filename}";
                        }
                        #endregion

                        _tenantDBContext.trn_cfscs.Add(confiscation);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var witnesses = new List<trn_witness>();
                        if (InputModel.witnesses != null && InputModel.witnesses.Count > 0)
                        {
                            foreach (var w in InputModel.witnesses)
                            {
                                var witness = new trn_witness();
                                witness.trn_id = confiscation.trn_cfsc_id;
                                witness.trn_type = "CONFISCATION";
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

                        #region Confiscated Item
                        var items = new List<trn_cfsc_item>();
                        int iin = 0;
                        if (InputModel.items != null && InputModel.items.Count > 0)
                        {
                            foreach (var ii in InputModel.items)
                            {
                                iin++;
                                var item = new trn_cfsc_item();
                                item.trn_cfsc_id = confiscation.trn_cfsc_id;
                                item.inv_id = ii.inv_id;
                                item.cnt_item = ii.cnt_item;
                                item.description = ii.description;
                                item.is_deleted = false;
                                item.creator_id = runUserID;
                                item.created_at = DateTime.Now;
                                items.Add(item);
                            }

                            if (items.Count > 0)
                            {
                                _tenantDBContext.trn_cfsc_items.AddRange(items);
                            }
                        }
                        #endregion

                        #region Proof Image
                        var proofs = new List<trn_cfsc_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(confiscation.cfsc_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(confiscation);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(confiscation);
                                var proof = new trn_cfsc_img();
                                proof.trn_cfsc_id = confiscation.trn_cfsc_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_cfsc_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        confiscation = await GeneratePdfTicket(confiscation, items, proofs);
                        _tenantDBContext.trn_cfscs.Update(confiscation);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();

                        var result = new
                        {
                            trn_cfsc_id = confiscation.trn_cfsc_id,
                            doc_name = confiscation.doc_name,
                            doc_pathurl = confiscation.doc_pathurl
                        };
                        return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta sitaan")));
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

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromForm] patrol_cfsc_input_model InputModel)
        {
            try
            {
                if (InputModel.witnesses == null || InputModel.witnesses.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["witnesses"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["witnesses"].ToString();
                        var fixedJson = rawItemReq;
                        if (!rawItemReq.StartsWith("[") || !rawItemReq.EndsWith("]"))
                        {
                            fixedJson = "[" + rawItemReq + "]";
                        }
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_cfsc_witness>>(fixedJson);
                    }
                }

                if (InputModel.items == null || InputModel.items.Count == 0)
                {
                    var Request = await HttpContext.Request.ReadFormAsync();
                    if (Request["items"] != StringValues.Empty)
                    {
                        var rawItemReq = Request["items"].ToString();
                        var fixedJson = rawItemReq;
                        if (!rawItemReq.StartsWith("[") || !rawItemReq.EndsWith("]"))
                        {
                            fixedJson = "[" + rawItemReq + "]";
                        }
                        InputModel.items = JsonConvert.DeserializeObject<List<patrol_cfsc_item_model>>(fixedJson);
                    }
                }

                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.cfsc_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Sitaan diperlukan")));
                }

                var confiscation = await _tenantDBContext.trn_cfscs.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);
                if (confiscation == null)
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
                        confiscation.owner_icno = InputModel.owner_icno;
                        confiscation.cfsc_ref_no = InputModel.cfsc_ref_no;
                        confiscation.instruction = InputModel.instruction;
                        confiscation.offs_location = InputModel.offs_location;
                        confiscation.scen_id = InputModel.scen_id;
                        confiscation.cfsc_longitude = InputModel.cfsc_longitude;
                        confiscation.cfsc_latitude = InputModel.cfsc_latitude;
                        confiscation.trnstatus_id = InputModel.trnstatus_id;
                        confiscation.license_id = InputModel.license_id;
                        confiscation.offense_code = InputModel.offense_code;
                        confiscation.uuk_code = InputModel.uuk_code;
                        confiscation.act_code = InputModel.act_code;
                        confiscation.section_code = InputModel.section_code;
                        confiscation.schedule_id = InputModel.schedule_id;
                        confiscation.tax_accno = InputModel.tax_accno;
                        confiscation.is_tax = InputModel.is_tax;
                        confiscation.user_id = InputModel.user_id;
                        confiscation.is_deleted = false;
                        confiscation.modifier_id = runUserID;
                        confiscation.modified_at = DateTime.Now;
                        //2025-04-08 - added new field
                        confiscation.recipient_name = InputModel.recipient_name;
                        confiscation.recipient_icno = InputModel.recipient_icno;
                        confiscation.recipient_telno = InputModel.recipient_telno;
                        confiscation.recipient_addr = InputModel.recipient_addr;
                        //2025-04-11 - add field for relationship
                        confiscation.recipient_relation_id = InputModel.recipient_relation_id;

                        #region receipient signature
                        //2025-04-08 - added new field
                        if (InputModel.recipient_sign != null)
                        {
                            string ImageUploadExt = Path.GetExtension(InputModel.recipient_sign.FileName).ToString().ToLower();
                            string Filename = $"{GetValidFilename(confiscation.cfsc_ref_no)}_receipient_signature{ImageUploadExt}";
                            var UploadPath = await getUploadPath(confiscation);
                            var Fullpath = Path.Combine(UploadPath, Filename);
                            using (var stream = new FileStream(Fullpath, FileMode.Create))
                            {
                                await InputModel.recipient_sign.CopyToAsync(stream);
                            }
                            string pathurl = await getViewUrl(confiscation);
                            confiscation.recipient_sign = $"{pathurl}/{Filename}";
                        }
                        #endregion

                        _tenantDBContext.trn_cfscs.Update(confiscation);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var existingWitness = await _tenantDBContext.trn_witnesses.Where(x => x.trn_type == "CONFISCATION" && x.trn_id == confiscation.trn_cfsc_id).ToListAsync();
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
                                witness.trn_id = confiscation.trn_cfsc_id;
                                witness.trn_type = "CONFISCATION";
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

                        #region Confiscated Item
                        var existingItems = await _tenantDBContext.trn_cfsc_items.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).ToListAsync();
                        if (existingItems != null)
                        {
                            _tenantDBContext.trn_cfsc_items.RemoveRange(existingItems);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var items = new List<trn_cfsc_item>();
                        int iin = 0;
                        if (InputModel.items != null && InputModel.items.Count > 0)
                        {
                            foreach (var ii in InputModel.items)
                            {
                                iin++;
                                var item = new trn_cfsc_item();
                                item.trn_cfsc_id = confiscation.trn_cfsc_id;
                                item.inv_id = ii.inv_id;
                                item.cnt_item = ii.cnt_item;
                                item.description = ii.description;
                                item.is_deleted = false;
                                item.creator_id = runUserID;
                                item.created_at = DateTime.Now;
                                items.Add(item);
                            }

                            if (items.Count > 0)
                            {
                                _tenantDBContext.trn_cfsc_items.AddRange(items);
                            }
                        }
                        #endregion

                        #region Proof Image
                        var existingProofs = await _tenantDBContext.trn_cfsc_imgs.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).ToListAsync();
                        if (existingProofs != null)
                        {
                            var UploadPath = await getUploadPath(confiscation);
                            foreach (var existingProof in existingProofs)
                            {
                                var Fullpath = Path.Combine(UploadPath, existingProof.filename);
                                await rmvExistFile(Fullpath);
                            }
                            _tenantDBContext.trn_cfsc_imgs.RemoveRange(existingProofs);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var proofs = new List<trn_cfsc_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(confiscation.cfsc_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(confiscation);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(confiscation);
                                var proof = new trn_cfsc_img();
                                proof.trn_cfsc_id = confiscation.trn_cfsc_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_cfsc_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        confiscation = await GeneratePdfTicket(confiscation, items, proofs);
                        _tenantDBContext.trn_cfscs.Update(confiscation);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();

                        var result = new
                        {
                            trn_cfsc_id = confiscation.trn_cfsc_id,
                            doc_name = confiscation.doc_name,
                            doc_pathurl = confiscation.doc_pathurl
                        };
                        return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai sitaan")));
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
                var formField = await _tenantDBContext.trn_cfscs.FirstOrDefaultAsync(x => x.trn_cfsc_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_cfscs.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Listing by specific field
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetConfiscationListByUserId(int UserId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.creator_id == UserId);

                var confiscation_lists = await (from n in initQuery
                                                where n.creator_id == UserId
                                                join ts in _tenantDBContext.ref_trn_statuses
                                                on n.trnstatus_id equals ts.status_id into tsg
                                                from ts in tsg.DefaultIfEmpty()
                                                select new
                                                {
                                                    n.trn_cfsc_id,
                                                    n.cfsc_ref_no,
                                                    n.section_code,
                                                    n.act_code,
                                                    n.created_at,
                                                    n.modified_at,
                                                    n.trnstatus_id,
                                                    n.doc_pathurl,
                                                    n.doc_name,
                                                    trnstatus_view = ts.status_name,
                                                }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = confiscation_lists.Count,
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

        [HttpGet("{ScheduleId}")]
        public async Task<IActionResult> GetConfiscationListBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var confiscation_lists = await (from n in _tenantDBContext.trn_cfscs
                                                where n.schedule_id == ScheduleId
                                                join ts in _tenantDBContext.ref_trn_statuses
                                                on n.trnstatus_id equals ts.status_id into tsg
                                                from ts in tsg.DefaultIfEmpty()
                                                select new
                                                {
                                                    n.trn_cfsc_id,
                                                    n.cfsc_ref_no,
                                                    n.section_code,
                                                    n.act_code,
                                                    n.created_at,
                                                    n.modified_at,
                                                    n.trnstatus_id,
                                                    n.doc_pathurl,
                                                    n.doc_name,
                                                    trnstatus_view = ts.status_name,
                                                }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = confiscation_lists.Count,
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

        [HttpGet("{TaxAccNo}")]
        public async Task<IActionResult> GetConfiscationListByTaxAccNo(string TaxAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.tax_accno == TaxAccNo);

                var confiscation_lists = await (from n in initQuery
                                                join ts in _tenantDBContext.ref_trn_statuses
                                                on n.trnstatus_id equals ts.status_id into tsg
                                                from ts in tsg.DefaultIfEmpty()
                                                select new
                                                {
                                                    n.trn_cfsc_id,
                                                    n.cfsc_ref_no,
                                                    n.section_code,
                                                    n.act_code,
                                                    n.created_at,
                                                    n.modified_at,
                                                    n.trnstatus_id,
                                                    n.doc_pathurl,
                                                    n.doc_name,
                                                    trnstatus_view = ts.status_name,
                                                }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = confiscation_lists.Count,
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

        [HttpGet("{LicenseAccNo}")]
        public async Task<IActionResult> GetConfiscationListByLicenseAccNo(string LicenseAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();
                var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.license_accno == LicenseAccNo);
                if (licenseInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == licenseInfo.licensee_id);

                var confiscation_lists = await (from n in initQuery
                                                join ts in _tenantDBContext.ref_trn_statuses
                                                on n.trnstatus_id equals ts.status_id into tsg
                                                from ts in tsg.DefaultIfEmpty()
                                                select new
                                                {
                                                    n.trn_cfsc_id,
                                                    n.cfsc_ref_no,
                                                    n.section_code,
                                                    n.act_code,
                                                    n.created_at,
                                                    n.modified_at,
                                                    n.trnstatus_id,
                                                    n.doc_pathurl,
                                                    n.doc_name,
                                                    trnstatus_view = ts.status_name,
                                                }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = confiscation_lists.Count,
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

        [HttpGet("{LicenseId}")]
        public async Task<IActionResult> GetConfiscationListByLicenseId(int LicenseId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == LicenseId);

                var confiscation_lists = await (from n in initQuery
                                                join ts in _tenantDBContext.ref_trn_statuses
                                                on n.trnstatus_id equals ts.status_id into tsg
                                                from ts in tsg.DefaultIfEmpty()
                                                select new
                                                {
                                                    n.trn_cfsc_id,
                                                    n.cfsc_ref_no,
                                                    n.section_code,
                                                    n.act_code,
                                                    n.created_at,
                                                    n.modified_at,
                                                    n.trnstatus_id,
                                                    n.doc_pathurl,
                                                    n.doc_name,
                                                    trnstatus_view = ts.status_name,
                                                }).ToListAsync();

                // Check if no record was found
                if (confiscation_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = confiscation_lists.Count,
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
        #endregion

        #region Count by specific field
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetConfiscationCountByUserId(int UserId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.creator_id == UserId);

                var resultData = await initQuery.CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{ScheduleId}")]
        public async Task<IActionResult> GetConfiscationCountBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = await _tenantDBContext.trn_cfscs.Where(x => x.schedule_id == ScheduleId).CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{TaxAccNo}")]
        public async Task<IActionResult> GetConfiscationCountByTaxAccNo(string TaxAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.tax_accno == TaxAccNo);

                var resultData = await initQuery.CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{LicenseAccNo}")]
        public async Task<IActionResult> GetConfiscationCountByLicenseAccNo(string LicenseAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.license_accno == LicenseAccNo);
                if (licenseInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == licenseInfo.licensee_id);

                var resultData = await initQuery.CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{LicenseId}")]
        public async Task<IActionResult> GetConfiscationCountByLicenseId(int LicenseId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_cfsc> initQuery = _tenantDBContext.trn_cfscs.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == LicenseId);

                var resultData = await initQuery.CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        #region Testing API
        // For Testing Purpose ONLY WIll be removed after finalization
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GeneratePDF(int ConsficationID)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                var confiscation = await _tenantDBContext.trn_cfscs.AsNoTracking().FirstOrDefaultAsync(x => x.trn_cfsc_id == ConsficationID);
                var items = await _tenantDBContext.trn_cfsc_items.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).AsNoTracking().ToListAsync();
                var proofs = await _tenantDBContext.trn_cfsc_imgs.Where(x => x.trn_cfsc_id == confiscation.trn_cfsc_id).AsNoTracking().ToListAsync();

                confiscation = await GeneratePdfTicket(confiscation, items, proofs);
                _tenantDBContext.trn_cfscs.Update(confiscation);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(confiscation, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta pdf")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        #region Private Logic
        private async Task<tenant_profile_pdf> GetTenantInfos()
        {
            tenant_profile_pdf tenantInfo = null;
            try
            {
                tenantInfo = await _dbContext.tenants.AsNoTracking().Select(x => new tenant_profile_pdf
                {
                    tenant_id = x.tenant_id,
                    tn_name = x.tn_name,
                    addr_line1 = x.addr_line1,
                    addr_line2 = x.addr_line2,
                    town_code = x.town_code,
                    district_code = x.district_code,
                    state_code = x.state_code,
                    country_code = x.country_code,
                    postcode = x.postcode,
                    tn_photo_filename = x.tn_photo_filename
                }).FirstOrDefaultAsync(x => x.tenant_id == _tenantId);

                if (tenantInfo.tn_photo_filename != null)
                {
                    var baseImageViewURL = await getBaseViewUrl("images");
                    var UploadPath = await getBaseUploadPath("images");

                    var Fullpath = Path.Combine(UploadPath, tenantInfo.tn_photo_filename);
                    tenantInfo.tn_photo_byte = GetPhysicalFileByte(Fullpath);

                    Fullpath = Path.Combine(UploadPath, "tiket_signature.png");
                    tenantInfo.tn_signature_byte = GetPhysicalFileByte(Fullpath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
            }
            finally
            {
                if (tenantInfo == null)
                {
                    tenantInfo = new tenant_profile_pdf
                    {
                        tn_name = "PBT PRO"
                    };

                }
            }

            return tenantInfo;
        }

        private async Task<trn_cfsc> GeneratePdfTicket(trn_cfsc record, List<trn_cfsc_item>? items, List<trn_cfsc_img>? proofs)
        {
            trn_cfsc result = record;
            try
            {
                var tenantInfo = await GetTenantInfos();
                var UploadPath = await getUploadPath(record);
                string Filename = $"{GetValidFilename(record.cfsc_ref_no)}.pdf";
                var Fullpath = Path.Combine(UploadPath, Filename);
                string Pathurl = await getViewUrl(record);
                #region Massage Data
                #region Main Ticket Data
                var initQuery = _tenantDBContext.trn_cfscs
                                .Where(t => t.trn_cfsc_id == record.trn_cfsc_id)
                                .GroupJoin(
                                    _tenantDBContext.ref_cfsc_scenarios,
                                    t => t.scen_id,
                                    d => d.scen_id,
                                    (t, gd) => new { trn_cfsc = t, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rdel) => new { jd.trn_cfsc, ref_deliver = rdel, mst_licensee = (mst_licensee)null, mst_taxtholder = (mst_taxholder)null, mst_owner = (mst_owner)null }
                                )
                                .GroupJoin(
                                    _tenantDBContext.ref_cfsc_scenarios,
                                    t => t.trn_cfsc.scen_id,
                                    d => d.scen_id,
                                    (jd, gd) => new { jd.trn_cfsc, jd.ref_deliver, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rnd) => new { jd.trn_cfsc, jd.ref_deliver, ref_notice_duration = rnd, mst_licensee = (mst_licensee)null, mst_taxtholder = (mst_taxholder)null, mst_owner = (mst_owner)null }
                                );

                if (record.is_tax == true)
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_taxholders,
                                    t => t.trn_cfsc.tax_accno,
                                    d => d.tax_accno,
                                    (t, gd) => new { t.trn_cfsc, t.ref_deliver, t.ref_notice_duration, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mth) => new { jd.trn_cfsc, jd.ref_deliver, jd.ref_notice_duration, mst_licensee = (mst_licensee)null, mst_taxtholder = mth }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_premis,
                                    t => t.mst_taxtholder.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_cfsc, t.ref_deliver, t.ref_notice_duration, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_cfsc, jd.ref_deliver, jd.ref_notice_duration, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
                                );
                }
                else
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_licensees,
                                    t => t.trn_cfsc.license_id,
                                    d => d.licensee_id,
                                    (t, gd) => new { t.trn_cfsc, t.ref_deliver, t.ref_notice_duration, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mli) => new { jd.trn_cfsc, jd.ref_deliver, jd.ref_notice_duration, mst_licensee = mli, mst_taxtholder = (mst_taxholder)null }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_licensees,
                                    t => t.mst_licensee.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_cfsc, t.ref_deliver, t.ref_notice_duration, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_cfsc, jd.ref_deliver, jd.ref_notice_duration, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
                                );
                }

                var ticketDet = await initQuery
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                #endregion

                #region Akta/Seksyen/UUK/KEsalahan
                var ticketASUO = await _dbContext.ref_law_offenses
                                .Where(t => t.offense_code == record.offense_code)
                                .GroupJoin(
                                    _dbContext.ref_law_acts,
                                    t => t.act_code,
                                    d => d.act_code,
                                    (t, gd) => new { t, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rla) => new { ref_law_offense = jd.t, ref_law_act = rla }
                                )
                                .GroupJoin(
                                    _dbContext.ref_law_sections,
                                    t => t.ref_law_offense.section_code,
                                    d => d.section_code,
                                    (t, gd) => new { t.ref_law_offense, t.ref_law_act, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rls) => new { jd.ref_law_offense, jd.ref_law_act, ref_law_section = rls }
                                ).GroupJoin(
                                    _dbContext.ref_law_uuks,
                                    t => t.ref_law_offense.uuk_code,
                                    d => d.uuk_code,
                                    (t, gd) => new { t.ref_law_offense, t.ref_law_act, t.ref_law_section, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rlu) => new { jd.ref_law_offense, jd.ref_law_act, jd.ref_law_section, ref_law_uuk = rlu }
                                )
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
                #endregion

                #region Items
                var joinItems = items
                    .GroupJoin(
                        _tenantDBContext.ref_cfsc_inventories,
                        item => item.inv_id,
                        inventory => inventory.inv_id,
                        (item, inventory) => new
                        {
                            itemInfo = item,
                            invInfo = inventory.DefaultIfEmpty().FirstOrDefault()
                        }
                    );

                var btmmItems = joinItems.Where(x => x.invInfo.item_type == 1).ToList();
                var bmmItems = joinItems.Where(x => x.invInfo.item_type == 2).ToList();

                #endregion
                #endregion

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        //page.Size(PageSizes.A4);
                        page.ContinuousSize(PageSizes.C9.Width);
                        page.Margin(5);
                        page.DefaultTextStyle(x => x.FontSize(3).LineHeight(1.8f));

                        page.Content()
                            .Column(column =>
                            {
                                column.Item().AlignCenter().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                    });

                                    if (tenantInfo?.tn_photo_byte?.Length > 0)
                                    {
                                        table.Cell().AlignCenter().PaddingBottom(5).Height(30).Image(tenantInfo.tn_photo_byte).FitArea();
                                    }
                                    else
                                    {
                                        table.Cell().AlignCenter().Height(30).Text($"   ").FontSize(4).Underline().Bold();
                                    }

                                    table.Cell().AlignCenter().PaddingBottom(5).Text($"{tenantInfo.tn_name.ToUpper()}").FontSize(4);
                                    table.Cell().AlignCenter().Text($"NOTIS PEMBERITAHUAN KESALAHAN").FontSize(4).Underline().Bold();
                                    table.Cell().AlignCenter().PaddingBottom(5).Text($"SERTA SITAAN").FontSize(4).Underline().Bold();
                                });

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                    });


                                    var message = "Jika tiada tuntutan dalam tempoh 1 bulan, barang-barang BTMM akan dilupuskan oleh pihak Majlis, " +
                                                  "manakala bagi barang-barang BMM pula pihak Majlis akan melupuskan dalam tempoh masa yang munsabah sekiranya " +
                                                  "tiada sebarang tuntutan dibuat.";

                                    table.Cell().PaddingBottom(5).Text(text =>
                                    {
                                        text.Justify();
                                        text.Span(message);
                                    });


                                    table.Cell().PaddingBottom(5).PaddingLeft(2).AlignLeft().Text($"MAKLUMAT PENERIMA").Bold();

                                    table.Cell().PaddingBottom(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Nama Pemilik :");
                                        table.Cell().AlignRight().Text($"{ticketDet.mst_owner.owner_name}");

                                        if (record.is_tax != true)
                                        {
                                            table.Cell().PaddingLeft(5).AlignLeft().Text("Nama Syarikat :");
                                            table.Cell().AlignRight().Text($"{ticketDet.mst_licensee.business_name}");
                                            table.Cell().PaddingLeft(5).AlignLeft().Text("No. Syarikat :");
                                            table.Cell().AlignRight().Text($"{ticketDet.mst_licensee.ssm_no}");
                                            table.Cell().PaddingLeft(5).AlignLeft().AlignMiddle().Text("Alamat :");
                                            table.Cell().AlignRight().AlignMiddle().Text($"{ticketDet.mst_licensee.business_addr}");
                                        }
                                        else
                                        {
                                            table.Cell().PaddingLeft(5).AlignLeft().Text("No K/P :");
                                            table.Cell().AlignRight().Text($"{ticketDet.mst_owner.owner_icno}");
                                            table.Cell().PaddingLeft(5).AlignLeft().Text("No Telefon :");
                                            table.Cell().AlignRight().Text($"{ticketDet.mst_owner.owner_telno}");
                                            table.Cell().PaddingLeft(5).AlignLeft().AlignMiddle().Text("Alamat :");
                                            table.Cell().AlignRight().AlignMiddle().Text($"{ticketDet.mst_taxtholder.alamat}");
                                        }
                                    });

                                    table.Cell().PaddingBottom(5).PaddingLeft(2).AlignLeft().Text($"MAKLUMAT KESALAHAN").Bold();


                                    table.Cell().PaddingBottom(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        string aktaKesalahan = ticketASUO.ref_law_uuk != null ? $"{ticketASUO.ref_law_uuk.uuk_code} {ticketASUO.ref_law_uuk.uuk_description}" : $"{ticketASUO.ref_law_act.act_code} {ticketASUO.ref_law_act.act_description}";

                                        string kodKesalahan = $"{ticketASUO.ref_law_section.section_name} {ticketASUO.ref_law_offense.offense_name}";

                                        table.Cell().PaddingLeft(5).AlignLeft().Text("No Sitaan :");
                                        table.Cell().AlignRight().Text($"{record.cfsc_ref_no}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Tarikh & Masa :");
                                        table.Cell().AlignRight().Text($"{record.created_at?.ToString("dd/MM/yyyy hh:mm:ss tt")}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Akta Kesalahan:");
                                        table.Cell().AlignRight().Text($"{aktaKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Kod Kesalahan :");
                                        table.Cell().AlignRight().Text($"{kodKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Butir-Butir Kesalahan :");
                                        table.Cell().AlignRight().Text($"{ticketASUO.ref_law_offense.offense_description}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Nota :");
                                        table.Cell().AlignRight().Text($"{record.instruction}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Cara Penyerahan :");
                                        table.Cell().AlignRight().Text($"Serahan Tangan");


                                        table.Cell().PaddingTop(5).AlignLeft().Text("Barang Tidak Mudah Musnah (BTMM)");
                                        table.Cell().PaddingTop(5).AlignRight().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn();
                                            });

                                            if (btmmItems.Count > 0)
                                            {
                                                foreach (var item in btmmItems)
                                                {
                                                    table.Cell().AlignRight().Text($"{item.invInfo.inv_name} - {item.itemInfo.cnt_item}");
                                                }
                                            }
                                        });

                                        table.Cell().PaddingTop(5).AlignLeft().Text("Barang Mudah Musnah (BMM)");
                                        table.Cell().PaddingTop(5).AlignRight().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn();
                                            });

                                            if (bmmItems.Count > 0)
                                            {
                                                foreach (var item in bmmItems)
                                                {
                                                    table.Cell().AlignRight().Text($"{item.invInfo.inv_name} - {item.itemInfo.cnt_item}");
                                                }
                                            }
                                        });


                                        table.Cell().PaddingTop(15).AlignCenter().Text("_________________________________");
                                        table.Cell().PaddingTop(15).AlignCenter().Text("_________________________________");
                                        table.Cell().AlignCenter().Text("(TANDATANGAN PEMILIK)");
                                        table.Cell().AlignCenter().Text("(TANDATANGAN KETUA OPERASI)");
                                    });
                                });
                            });
                    });
                });

                document.GeneratePdf(Fullpath);


                result.doc_pathurl = $"{Pathurl}/{Filename}";
                result.doc_name = Filename;
            }
            catch (Exception ex)
            {
                result = record;
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                throw;
            }
            return record;
        }

        private async Task<string?> getUploadPath(trn_cfsc? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");

            result = await getBaseUploadPath("consfication", stringDate);

            return result;
        }

        private async Task<string?> getViewUrl(trn_cfsc? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");
            result = await getBaseViewUrl("consfication", stringDate);
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
        #endregion
    }
}
