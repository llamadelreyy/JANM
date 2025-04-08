/*
Project: PBT Pro
Description: Notices API controller to handle notices Form Field
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
    public class NoticesController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly string _feature = "NOTICES"; // follow module name (will be used in logging result to user)
        private readonly ILogger<NoticesController> _logger;
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };

        public NoticesController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<NoticesController> logger, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_notice>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_notices.AsNoTracking().ToListAsync();
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
                var notice = await _tenantDBContext.trn_notices.FirstOrDefaultAsync(x => x.trn_notice_id == Id);

                if (notice == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var result = MapEntity<patrol_notice_view_model>(notice);
                result.proofs = await _tenantDBContext.trn_notice_imgs.Where(x => x.trn_notice_id == notice.trn_notice_id).AsNoTracking().ToListAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] patrol_notice_input_model InputModel)
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
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_notice_witness>>(fixedJson);
                    }
                }

                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.notice_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Notis diperlukan")));
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
                        trn_notice notice = new trn_notice
                        {
                            owner_icno = InputModel.owner_icno,
                            notice_ref_no = InputModel.notice_ref_no,
                            instruction = InputModel.instruction,
                            offs_location = InputModel.offs_location,
                            duration_id = InputModel.duration_id,
                            deliver_id = InputModel.deliver_id,
                            notice_longitude = InputModel.notice_longitude,
                            notice_latitude = InputModel.notice_latitude,
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
                        };

                        #region receipient signature
                        //2025-04-08 - added new field
                        if (InputModel.recipient_sign != null)
                        {
                            string ImageUploadExt = Path.GetExtension(InputModel.recipient_sign.FileName).ToString().ToLower();
                            string Filename = $"{GetValidFilename(notice.notice_ref_no)}_receipient_signature{ImageUploadExt}";
                            var UploadPath = await getUploadPath(notice);
                            var Fullpath = Path.Combine(UploadPath, Filename);
                            using (var stream = new FileStream(Fullpath, FileMode.Create))
                            {
                                await InputModel.recipient_sign.CopyToAsync(stream);
                            }
                            string pathurl = await getViewUrl(notice);
                            notice.recipient_sign = $"{pathurl}/{Filename}";
                        }
                        #endregion

                        _tenantDBContext.trn_notices.Add(notice);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var witnesses = new List<trn_witness>();
                        if (InputModel.witnesses != null && InputModel.witnesses.Count > 0)
                        {
                            foreach (var w in InputModel.witnesses)
                            {
                                var witness = new trn_witness();
                                witness.trn_id = notice.trn_notice_id;
                                witness.trn_type = "NOTICE";
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
                        var proofs = new List<trn_notice_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(notice.notice_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(notice);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(notice);
                                var proof = new trn_notice_img();
                                proof.trn_notice_id = notice.trn_notice_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_notice_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        notice = await GeneratePdfTicket(notice, proofs);
                        _tenantDBContext.trn_notices.Update(notice);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();

                        var result = new
                        {
                            trn_notice_id = notice.trn_notice_id,
                            doc_name = notice.doc_name,
                            doc_pathurl = notice.doc_pathurl
                        };
                        return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta notis")));
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
        public async Task<IActionResult> Update(int Id, [FromForm] patrol_notice_input_model InputModel)
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
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_notice_witness>>(fixedJson);
                    }
                }

                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.notice_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Notis diperlukan")));
                }

                var notice = await _tenantDBContext.trn_notices.FirstOrDefaultAsync(x => x.trn_notice_id == Id);
                if (notice == null)
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
                        notice.owner_icno = InputModel.owner_icno;
                        notice.notice_ref_no = InputModel.notice_ref_no;
                        notice.instruction = InputModel.instruction;
                        notice.offs_location = InputModel.offs_location;
                        notice.duration_id = InputModel.duration_id;
                        notice.deliver_id = InputModel.deliver_id;
                        notice.notice_longitude = InputModel.notice_longitude;
                        notice.notice_latitude = InputModel.notice_latitude;
                        notice.trnstatus_id = InputModel.trnstatus_id;
                        notice.license_id = InputModel.license_id;
                        notice.offense_code = InputModel.offense_code;
                        notice.uuk_code = InputModel.uuk_code;
                        notice.act_code = InputModel.act_code;
                        notice.section_code = InputModel.section_code;
                        notice.tax_accno = InputModel.tax_accno;
                        notice.is_tax = InputModel.is_tax;
                        notice.modifier_id = runUserID;
                        notice.modified_at = DateTime.Now;

                        //2025-04-08 - added new field
                        notice.recipient_name = InputModel.recipient_name;
                        notice.recipient_icno = InputModel.recipient_icno;
                        notice.recipient_telno = InputModel.recipient_telno;
                        notice.recipient_addr = InputModel.recipient_addr;

                        #region receipient signature
                        //2025-04-08 - added new field
                        if (InputModel.recipient_sign != null)
                        {
                            string ImageUploadExt = Path.GetExtension(InputModel.recipient_sign.FileName).ToString().ToLower();
                            string Filename = $"{GetValidFilename(notice.notice_ref_no)}_receipient_signature{ImageUploadExt}";
                            var UploadPath = await getUploadPath(notice);
                            var Fullpath = Path.Combine(UploadPath, Filename);
                            using (var stream = new FileStream(Fullpath, FileMode.Create))
                            {
                                await InputModel.recipient_sign.CopyToAsync(stream);
                            }
                            string pathurl = await getViewUrl(notice);
                            notice.recipient_sign = $"{pathurl}/{Filename}";
                        }
                        #endregion

                        _tenantDBContext.trn_notices.Update(notice);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var existingWitness = await _tenantDBContext.trn_witnesses.Where(x => x.trn_type == "NOTICE" && x.trn_id == notice.trn_notice_id).ToListAsync();
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
                                witness.trn_id = notice.trn_notice_id;
                                witness.trn_type = "NOTICE";
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
                        var existingProofs = await _tenantDBContext.trn_notice_imgs.Where(x => x.trn_notice_id == notice.trn_notice_id).ToListAsync();
                        if (existingProofs != null)
                        {
                            var UploadPath = await getUploadPath(notice);
                            foreach (var existingProof in existingProofs)
                            {
                                var Fullpath = Path.Combine(UploadPath, existingProof.filename);
                                await rmvExistFile(Fullpath);
                            }
                            _tenantDBContext.trn_notice_imgs.RemoveRange(existingProofs);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var proofs = new List<trn_notice_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(notice.notice_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(notice);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(notice);
                                var proof = new trn_notice_img();
                                proof.trn_notice_id = notice.trn_notice_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_notice_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        notice = await GeneratePdfTicket(notice, proofs);
                        _tenantDBContext.trn_notices.Update(notice);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();

                        var result = new
                        {
                            trn_notice_id = notice.trn_notice_id,
                            doc_name = notice.doc_name,
                            doc_pathurl = notice.doc_pathurl
                        };
                        return Ok(result, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai notis")));
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
                var formField = await _tenantDBContext.trn_notices.FirstOrDefaultAsync(x => x.trn_notice_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_notices.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang notis")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Listing by specific field
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetNoticeListByUserId(int UserId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.creator_id == UserId);

                var notice_lists = await (from n in initQuery
                                          join ts in _tenantDBContext.ref_trn_statuses
                                          on n.trnstatus_id equals ts.status_id into tsg
                                          from ts in tsg.DefaultIfEmpty()
                                          select new
                                          {
                                              n.trn_notice_id,
                                              n.notice_ref_no,
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
                if (notice_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = notice_lists.Count,
                    notice_lists,
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
        public async Task<IActionResult> GetNoticeListBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var notice_lists = await (from n in _tenantDBContext.trn_notices
                                          where n.schedule_id == ScheduleId
                                          join ts in _tenantDBContext.ref_trn_statuses
                                          on n.trnstatus_id equals ts.status_id into tsg
                                          from ts in tsg.DefaultIfEmpty()
                                          select new
                                          {
                                              n.trn_notice_id,
                                              n.notice_ref_no,
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
                if (notice_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = notice_lists.Count,
                    notice_lists,
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
        public async Task<IActionResult> GetNoticeListByTaxAccNo(string TaxAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.tax_accno == TaxAccNo);

                var notice_lists = await (from n in initQuery
                                          join ts in _tenantDBContext.ref_trn_statuses
                                          on n.trnstatus_id equals ts.status_id into tsg
                                          from ts in tsg.DefaultIfEmpty()
                                          select new
                                          {
                                              n.trn_notice_id,
                                              n.notice_ref_no,
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
                if (notice_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = notice_lists.Count,
                    notice_lists,
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
        public async Task<IActionResult> GetNoticeListByLicenseAccNo(string LicenseAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();
                var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.license_accno == LicenseAccNo);
                if (licenseInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == licenseInfo.licensee_id);

                var notice_lists = await (from n in initQuery
                                          join ts in _tenantDBContext.ref_trn_statuses
                                          on n.trnstatus_id equals ts.status_id into tsg
                                          from ts in tsg.DefaultIfEmpty()
                                          select new
                                          {
                                              n.trn_notice_id,
                                              n.notice_ref_no,
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
                if (notice_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = notice_lists.Count,
                    notice_lists,
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
        public async Task<IActionResult> GetNoticeListByLicenseId(int LicenseId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var resultData = new List<dynamic>();

                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initQuery = initQuery.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                initQuery = initQuery.Where(x => x.license_id == LicenseId);

                var notice_lists = await (from n in initQuery
                                          join ts in _tenantDBContext.ref_trn_statuses
                                          on n.trnstatus_id equals ts.status_id into tsg
                                          from ts in tsg.DefaultIfEmpty()
                                          select new
                                          {
                                              n.trn_notice_id,
                                              n.notice_ref_no,
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
                if (notice_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = notice_lists.Count,
                    notice_lists,
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
        public async Task<IActionResult> GetNoticeCountByUserId(int UserId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

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
        public async Task<IActionResult> GetNoticeCountBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = await _tenantDBContext.trn_notices.Where(x => x.schedule_id == ScheduleId).CountAsync();
                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{TaxAccNo}")]
        public async Task<IActionResult> GetNoticeCountByTaxAccNo(string TaxAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

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
        public async Task<IActionResult> GetNoticeCountByLicenseAccNo(string LicenseAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var licenseInfo = await _tenantDBContext.mst_licensees.AsNoTracking().FirstOrDefaultAsync(x => x.license_accno == LicenseAccNo);
                if (licenseInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

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
        public async Task<IActionResult> GetNoticeCountByLicenseId(int LicenseId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_notice> initQuery = _tenantDBContext.trn_notices.AsNoTracking();

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

        #region Report
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_notices_view>>> ListReport()
        {
            try
            {
                var tenantNotices = await _tenantDBContext.trn_notices
                   .AsNoTracking()
                   .ToListAsync();

                var tenantOwners = (await _tenantDBContext.mst_owner_licensees
                    .Where(mol => !string.IsNullOrEmpty(mol.owner_icno))
                    .Select(mol => new { mol.owner_icno, mol.owner_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(mol => mol.owner_icno, mol => mol);

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

                var lawOffenses = (await _dbContext.ref_law_offenses
                    .Select(lo => new { lo.offense_code, lo.offense_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(lo => lo.offense_code, lo => lo.offense_name);

                var lawActs = (await _dbContext.ref_law_acts
                    .Where(rla => rla.act_code != null)
                    .Select(rla => new { rla.act_code, rla.act_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(rla => rla.act_code, rla => rla.act_name);

                var lawSections = (await _dbContext.ref_law_sections
                    .Where(rls => rls.section_code != null)
                    .Select(rls => new { rls.section_code, rls.section_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(rls => rls.section_code, rls => rls.section_name);

                var lawUuks = (await _dbContext.ref_law_uuks
                    .Where(rlu => rlu.uuk_code != null)
                    .Select(rlu => new { rlu.uuk_code, rlu.uuk_name })
                    .AsNoTracking()
                    .ToListAsync())
                    .ToDictionary(rlu => rlu.uuk_code, rlu => rlu.uuk_name);

                var tenantDelivers = (await _tenantDBContext.ref_delivers
                   .Select(rd => new { rd.deliver_id, rd.deliver_name })
                   .AsNoTracking()
                   .ToListAsync())
                   .ToDictionary(rd => rd.deliver_id, rd => rd.deliver_name);

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

                var tenantNoticeImgs = (await _tenantDBContext.trn_notice_imgs
                    .Select(img => new { img.trn_notice_id, img.pathurl })
                    .AsNoTracking()
                    .ToListAsync())
                    .GroupBy(img => img.trn_notice_id)
                    .ToDictionary(g => g.Key, g => g.Select(img => img.pathurl).Distinct().ToList());

                //var tenantNoticeDurations = (await _tenantDBContext.ref_notice_durations
                //  .Select(nd => new { nd.duration_id, nd.duration_value })
                //  .AsNoTracking()
                //  .ToListAsync())
                //  .ToDictionary(nd => nd.duration_id, nd => nd.duration_value);

                var tenantNoticeDurations = await _tenantDBContext.ref_notice_durations
                   .Select(nd => new { nd.duration_id, nd.duration_value })
                   .AsNoTracking()
                   .ToListAsync();

                var results = tenantNotices.Select(tn =>
                {
                    tenantOwners.TryGetValue(tn.owner_icno, out var owner);
                    //tenantLicensees.TryGetValue((int)tn?.license_id, out var licensee);
                    lawOffenses.TryGetValue(tn?.offense_code, out var offense);
                    tenantPatrolSchedules.TryGetValue((int)tn.schedule_id, out var officerId);
                    users.TryGetValue(officerId, out var officer);
                    tenantNoticeImgs.TryGetValue(tn.trn_notice_id, out var images);
                    tenantDelivers.TryGetValue((int)tn.deliver_id, out var deliver);
                    tenantWitness.TryGetValue(tn.trn_notice_id, out var witnesses);
                    //tenantNoticeDurations.TryGetValue((int)tn?.duration_id, out var duration);

                    var licensee = tn?.license_id.HasValue == true
                    ? tenantLicensees.GetValueOrDefault(tn.license_id.Value)
                    : null;

                    var law = !string.IsNullOrEmpty(tn?.act_code)
                    ? lawActs.GetValueOrDefault(tn.act_code, "")
                    : "";

                    var section = !string.IsNullOrEmpty(tn?.section_code)
                        ? lawSections.GetValueOrDefault(tn.section_code, "")
                        : "";

                    var uuk = !string.IsNullOrEmpty(tn?.uuk_code)
                        ? lawUuks.GetValueOrDefault(tn.uuk_code, "")
                        : "";

                    return new trn_notices_view
                    {
                        id_notis = tn.trn_notice_id,
                        no_lesen = licensee?.license_accno,
                        nama_perniagaan = licensee?.business_name,
                        nama_pemilik = owner?.owner_name,
                        no_rujukan = tn?.notice_ref_no,
                        status_notis_id = tn?.trnstatus_id,
                        status_notis = tenantStatuses.FirstOrDefault(s => s.status_id == tn.trnstatus_id)?.status_name,
                        kod_kesalahan = offense,
                        akta_kesalahan = law ?? "",
                        kod_seksyen = section ?? "",
                        kod_uuk = uuk ?? "",
                        arahan = tn?.instruction,
                        lokasi_kesalahan = tn?.offs_location ?? "",
                        TarikhData = tn?.created_at,
                        nama_pegawai = officer,
                        ssm_no = licensee?.ssm_no,
                        alamat_perniagaan = licensee?.business_addr,
                        nama_dokumen = tn?.doc_name ?? null,
                        pautan_dokumen = tn?.doc_pathurl ?? null,
                        imej_notis = images ?? new List<string>(),
                        cara_serahan = deliver,
                        nama_saksi = witnesses != null && witnesses.Any() ? string.Join(", ", witnesses) : "",
                        tarikh_tamat = CalculateTarikhTamat((DateTime)tn.created_at, tenantNoticeDurations.FirstOrDefault(s => s.duration_id == tn.duration_id)?.duration_value),
                        no_cukai = tn?.tax_accno ?? "",
                        tempoh_notis_id = tn?.duration_id,
                        tempoh_notis = tenantNoticeDurations.FirstOrDefault(s => s.duration_id == tn.duration_id)?.duration_value,
                        lesen_id = tn?.license_id ?? null
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
        public async Task<IActionResult> UpdateReport(int Id, [FromBody] trn_notices_view InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.trn_notices.FirstOrDefaultAsync(x => x.trn_notice_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                #endregion

                formField.trnstatus_id = InputModel.status_notis_id;
                formField.duration_id = InputModel.tempoh_notis_id;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.trn_notices.Update(formField);
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
                var formField = await _tenantDBContext.trn_notices.FirstOrDefaultAsync(x => x.trn_notice_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _tenantDBContext.trn_notices.Remove(formField);
                    await _tenantDBContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    formField.is_deleted = true;
                    formField.modifier_id = runUserID;
                    formField.modified_at = DateTime.Now;

                    _tenantDBContext.trn_notices.Update(formField);
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
        #endregion

        #region Testing API
        // For Testing Purpose ONLY WIll be removed after finalization
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GeneratePDF(int NoticeID)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                var notice = await _tenantDBContext.trn_notices.AsNoTracking().FirstOrDefaultAsync(x => x.trn_notice_id == NoticeID);
                var proofs = await _tenantDBContext.trn_notice_imgs.Where(x => x.trn_notice_id == notice.trn_notice_id).AsNoTracking().ToListAsync();

                notice = await GeneratePdfTicket(notice, proofs);
                _tenantDBContext.trn_notices.Update(notice);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(notice, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta pdf")));
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

        private async Task<trn_notice> GeneratePdfTicket(trn_notice record, List<trn_notice_img>? proofs)
        {
            trn_notice result = record;
            try
            {
                var tenantInfo = await GetTenantInfos();
                var UploadPath = await getUploadPath(record);
                string Filename = $"{GetValidFilename(record.notice_ref_no)}.pdf";
                var Fullpath = Path.Combine(UploadPath, Filename);
                string Pathurl = await getViewUrl(record);
                #region Massage Data
                var initQuery = _tenantDBContext.trn_notices
                                .Where(t => t.trn_notice_id == record.trn_notice_id)
                                .GroupJoin(
                                    _tenantDBContext.ref_delivers,
                                    t => t.deliver_id,
                                    d => d.deliver_id,
                                    (t, gd) => new { trn_notice = t, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rdel) => new { jd.trn_notice, ref_deliver = rdel, mst_licensee = (mst_licensee)null, mst_taxtholder = (mst_taxholder)null, mst_owner = (mst_owner)null }
                                )
                                .GroupJoin(
                                    _tenantDBContext.ref_notice_durations,
                                    t => t.trn_notice.duration_id,
                                    d => d.duration_id,
                                    (jd, gd) => new { jd.trn_notice, jd.ref_deliver, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rnd) => new { jd.trn_notice, jd.ref_deliver, ref_notice_duration = rnd, mst_licensee = (mst_licensee)null, mst_taxtholder = (mst_taxholder)null, mst_owner = (mst_owner)null }
                                );

                if (record.is_tax == true)
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_taxholders,
                                    t => t.trn_notice.tax_accno,
                                    d => d.tax_accno,
                                    (t, gd) => new { t.trn_notice, t.ref_deliver, t.ref_notice_duration, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mth) => new { jd.trn_notice, jd.ref_deliver, jd.ref_notice_duration, mst_licensee = (mst_licensee)null, mst_taxtholder = mth }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_premis,
                                    t => t.mst_taxtholder.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_notice, t.ref_deliver, t.ref_notice_duration, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_notice, jd.ref_deliver, jd.ref_notice_duration, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
                                );
                }
                else
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_licensees,
                                    t => t.trn_notice.license_id,
                                    d => d.licensee_id,
                                    (t, gd) => new { t.trn_notice, t.ref_deliver, t.ref_notice_duration, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mli) => new { jd.trn_notice, jd.ref_deliver, jd.ref_notice_duration, mst_licensee = mli, mst_taxtholder = (mst_taxholder)null }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_licensees,
                                    t => t.mst_licensee.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_notice, t.ref_deliver, t.ref_notice_duration, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_notice, jd.ref_deliver, jd.ref_notice_duration, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
                                );
                }

                var ticketDet = await initQuery
                                .AsNoTracking()
                                .FirstOrDefaultAsync();


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
                                    table.Cell().AlignCenter().PaddingBottom(5).Text($"NOTIS PEMBERITAHUAN KESALAHAN").FontSize(4).Underline().Bold();
                                });

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                    });


                                    var message = "Dimaklumkan bahawa hasil pemeriksaan yang dijalankan mendapati tuan/puan telah melakukan kesalahan " +
                                                  $"di bawah {ticketASUO.ref_law_uuk.uuk_name ?? ticketASUO.ref_law_act.act_name} iaitu {ticketASUO.ref_law_offense.offense_name} " +
                                                  "yang dikeluarkan oleh Pihak Berkuasa Melesen di lokasi kejadian seperti alamat di atas.\r\n\r\n" +
                                                  "Kegagalan tuan/puan mematuhi arahan di atas, boleh didenda sebanyak RM500.00 (Ringgit Malaysia Lima Ratus " +
                                                  "sahaja) atau kali kemudiannya denda sebanyak RM1,000.00 (Ringgit Malaysia Seribu) atau pihak Majlis " +
                                                  $"boleh memindah dan menahan halangan sehingga belanja dibayar kepada pihak Majlis di bawah {ticketASUO.ref_law_section.section_name} Akta yang sama.";


                                    table.Cell().PaddingBottom(5).Text(text => {
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

                                        table.Cell().PaddingLeft(5).AlignLeft().Text("No Rujukan Notis :");
                                        table.Cell().AlignRight().Text($"{record.notice_ref_no}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Tarikh & Masa :");
                                        table.Cell().AlignRight().Text($"{record.created_at?.ToString("dd/MM/yyyy hh:mm:ss tt")}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Akta Kesalahan:");
                                        table.Cell().AlignRight().Text($"{aktaKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Kod Kesalahan :");
                                        table.Cell().AlignRight().Text($"{kodKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Butir-Butir Kesalahan :");
                                        table.Cell().AlignRight().Text($"{ticketASUO.ref_law_offense.offense_description}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Tempoh Notis :");
                                        table.Cell().AlignRight().Text($"{ticketDet.ref_notice_duration.duration_value}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Arahan :");
                                        table.Cell().AlignRight().Text($"{record.instruction}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Cara Penyerahan :");
                                        table.Cell().AlignRight().Text($"{ticketDet.ref_deliver.deliver_name}");
                                    });

                                    table.Cell().PaddingTop(5).AlignLeft().Text("_________________________________");
                                    table.Cell().AlignLeft().Text("(TANDATANGAN PENERIMA)");

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

        private async Task<string?> getUploadPath(trn_notice? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");

            result = await getBaseUploadPath("notice", stringDate);

            return result;
        }

        private async Task<string?> getViewUrl(trn_notice? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");
            result = await getBaseViewUrl("notice", stringDate);
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

        private DateTime CalculateTarikhTamat(DateTime createdAt, string? duration)
        {

            DateTime tarikhTamat;

            switch (duration)
            {
                case "SERTA MERTA":
                    tarikhTamat = createdAt;
                    break;

                case "3 HARI":
                    tarikhTamat = createdAt.AddDays(3);
                    break;

                case "5 HARI":
                    tarikhTamat = createdAt.AddDays(5);
                    break;

                case "7 HARI":
                    tarikhTamat = createdAt.AddDays(7);
                    break;

                case "14 HARI":
                    tarikhTamat = createdAt.AddDays(14);
                    break;

                case "30 HARI":
                    tarikhTamat = createdAt.AddDays(30);
                    break;

                default:
                    tarikhTamat = createdAt;
                    break;
            }
            return tarikhTamat;
        }

        #endregion

    }
}
