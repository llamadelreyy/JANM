/*
Project: PBT Pro
Description: Compounds API controller to handle compounds Form Field
Author: Fakhrul
Date: January 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
20/02/2025 - revamp table & logic
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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CompoundsController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IMapBarcodeService _mapBarcodeService;
        private readonly string _feature = "COMPOUNDS"; // follow module name (will be used in logging result to user)
        private readonly ILogger<CompoundsController> _logger;
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };

        public CompoundsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<CompoundsController> logger, IMapBarcodeService mapBarcodeService, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _configuration = configuration;
            _mapBarcodeService = mapBarcodeService;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_compound>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_cmpds.AsNoTracking().ToListAsync();
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
                var compound = await _tenantDBContext.trn_cmpds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);

                if (compound == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                var result = MapEntity<patrol_cmpd_view_model>(compound);
                result.proofs = await _tenantDBContext.trn_cmpd_imgs.Where(x => x.trn_cmpd_id == compound.trn_cmpd_id).AsNoTracking().ToListAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] patrol_cmpd_input_model InputModel)
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
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_cmpd_witness>>(fixedJson);
                    }
                }

                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.cmpd_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Kompaun diperlukan")));
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
                        trn_cmpd compound = new trn_cmpd
                        {
                            owner_icno = InputModel.owner_icno,
                            cmpd_ref_no = InputModel.cmpd_ref_no,
                            instruction = InputModel.instruction,
                            offs_location = InputModel.offs_location,
                            amt_cmpd = InputModel.amt_cmpd,
                            deliver_id = InputModel.deliver_id,
                            cmpd_longitude = InputModel.cmpd_longitude,
                            cmpd_latitude = InputModel.cmpd_latitude,
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
                        };

                        _tenantDBContext.trn_cmpds.Add(compound);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var witnesses = new List<trn_witness>();
                        if (InputModel.witnesses != null && InputModel.witnesses.Count > 0)
                        {
                            foreach (var w in InputModel.witnesses)
                            {
                                var witness = new trn_witness();
                                witness.trn_id = compound.trn_cmpd_id;
                                witness.trn_type = "COMPOUND";
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
                        var proofs = new List<trn_cmpd_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(compound.cmpd_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(compound);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(compound);
                                var proof = new trn_cmpd_img();
                                proof.trn_cmpd_id = compound.trn_cmpd_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_cmpd_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        compound = await GeneratePdfTicket(compound, proofs);
                        _tenantDBContext.trn_cmpds.Update(compound);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();
                        return Ok(compound, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta kompaun")));
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
        public async Task<IActionResult> Update(int Id, [FromForm] patrol_cmpd_input_model InputModel)
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
                        InputModel.witnesses = JsonConvert.DeserializeObject<List<patrol_cmpd_witness>>(fixedJson);
                    }
                }

                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.cmpd_ref_no))
                {
                    return Error("", SystemMesg(_feature, "REFNO_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan No Kompaun diperlukan")));
                }

                var compound = await _tenantDBContext.trn_cmpds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);
                if (compound == null)
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
                        compound.owner_icno = InputModel.owner_icno;
                        compound.cmpd_ref_no = InputModel.cmpd_ref_no;
                        compound.instruction = InputModel.instruction;
                        compound.offs_location = InputModel.offs_location;
                        compound.amt_cmpd = InputModel.amt_cmpd;
                        compound.deliver_id = InputModel.deliver_id;
                        compound.cmpd_longitude = InputModel.cmpd_longitude;
                        compound.cmpd_latitude = InputModel.cmpd_latitude;
                        compound.trnstatus_id = InputModel.trnstatus_id;
                        compound.license_id = InputModel.license_id;
                        compound.offense_code = InputModel.offense_code;
                        compound.uuk_code = InputModel.uuk_code;
                        compound.act_code = InputModel.act_code;
                        compound.section_code = InputModel.section_code;
                        compound.tax_accno = InputModel.tax_accno;
                        compound.is_tax = InputModel.is_tax;
                        compound.modifier_id = runUserID;
                        compound.modified_at = DateTime.Now;

                        _tenantDBContext.trn_cmpds.Update(compound);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        #region Witness
                        var existingWitness = await _tenantDBContext.trn_witnesses.Where(x => x.trn_type == "COMPOUND" && x.trn_id == compound.trn_cmpd_id).ToListAsync();
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
                                witness.trn_id = compound.trn_cmpd_id;
                                witness.trn_type = "COMPOUND";
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
                        var existingProofs = await _tenantDBContext.trn_cmpd_imgs.Where(x => x.trn_cmpd_id == compound.trn_cmpd_id).ToListAsync();
                        if (existingProofs != null)
                        {
                            var UploadPath = await getUploadPath(compound);
                            foreach (var existingProof in existingProofs)
                            {
                                var Fullpath = Path.Combine(UploadPath, existingProof.filename);
                                await rmvExistFile(Fullpath);
                            }
                            _tenantDBContext.trn_cmpd_imgs.RemoveRange(existingProofs);
                            await _tenantDBContext.SaveChangesAsync();
                        }

                        var proofs = new List<trn_cmpd_img>();
                        int pfn = 0;
                        if (InputModel.proofs != null && InputModel.proofs.Count > 0)
                        {
                            foreach (var ip in InputModel.proofs)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{GetValidFilename(compound.cmpd_ref_no)}_proof_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath(compound);
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                string pathurl = await getViewUrl(compound);
                                var proof = new trn_cmpd_img();
                                proof.trn_cmpd_id = compound.trn_cmpd_id;
                                proof.filename = Filename;
                                proof.pathurl = $"{pathurl}/{Filename}";
                                proof.is_deleted = false;
                                proof.creator_id = runUserID;
                                proof.created_at = DateTime.Now;
                                proofs.Add(proof);
                            }

                            if (proofs.Count > 0)
                            {
                                _tenantDBContext.trn_cmpd_imgs.AddRange(proofs);
                            }
                        }
                        #endregion

                        #region PDF Ticket
                        compound = await GeneratePdfTicket(compound, proofs);
                        _tenantDBContext.trn_cmpds.Update(compound);
                        await _tenantDBContext.SaveChangesAsync();
                        #endregion

                        await transaction.CommitAsync();
                        return Ok(compound, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai kompaun")));
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
                var formField = await _tenantDBContext.trn_cmpds.FirstOrDefaultAsync(x => x.trn_cmpd_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.trn_cmpds.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang kompaun")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetCompoundListByUserId(int UserId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var compound_lists = await (from n in _tenantDBContext.trn_cmpds
                                            where n.creator_id == UserId
                                            select new
                                            {
                                                n.cmpd_ref_no,
                                                n.section_code,
                                                n.act_code,
                                                n.created_at,
                                                n.modified_at,
                                            }).ToListAsync();

                // Check if no record was found
                if (compound_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = compound_lists.Count,
                    compound_lists,
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
        public async Task<IActionResult> GetCompoundListBySchedId(int ScheduleId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var compound_lists = await (from n in _tenantDBContext.trn_cmpds
                                            where n.schedule_id == ScheduleId
                                            select new
                                            {
                                                n.cmpd_ref_no,
                                                n.section_code,
                                                n.act_code,
                                                n.created_at,
                                                n.modified_at,
                                            }).ToListAsync();

                if (compound_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = compound_lists.Count,
                    compound_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_compound>>> ListsAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_cmpds.AsNoTracking().ToListAsync();
                var dataPegawai = await _tenantDBContext.mst_patrol_schedules.AsNoTracking().ToListAsync();
                var dataPemilikLesen = await _tenantDBContext.mst_owner_licensees.AsNoTracking().ToListAsync();
                var dataMaklumatLesen = (from pemilik in dataPemilikLesen
                                         join lesen in _tenantDBContext.mst_licensees
                                         on pemilik.owner_icno equals lesen.owner_icno
                                         select new { pemilik, lesen }).ToList();

                var dataSaksi = await _tenantDBContext.trn_witnesses.AsNoTracking().ToListAsync();
               
                

                var result = (from cmpds in data
                              
                              join owner in dataMaklumatLesen on cmpds.owner_icno equals owner.pemilik.owner_icno
                              join officer in dataPegawai on cmpds.schedule_id equals officer.schedule_id
                              join status in _tenantDBContext.ref_trn_statuses on cmpds.trnstatus_id equals status.status_id
                              join acts in _dbContext.ref_law_acts on cmpds.act_code equals acts.act_code
                              join off in _dbContext.ref_law_offenses on cmpds.offense_code equals off.offense_code
                              join sec in _dbContext.ref_law_sections on cmpds.section_code equals sec.section_code
                              join del in _tenantDBContext.ref_delivers on cmpds.deliver_id equals del.deliver_id
                              join uuk in _dbContext.ref_law_uuks on cmpds.uuk_code equals uuk.uuk_code
                              join user in _dbContext.Users on officer.idno equals user.IdNo


                              select new trn_compound_view
                              {
                                  id_kompaun = cmpds.trn_cmpd_id,
                                  no_lesen = owner.lesen.license_accno,
                                  nama_perniagaan = owner.lesen.business_name,
                                  nama_pemilik = owner.pemilik.owner_name,
                                  no_rujukan = cmpds.cmpd_ref_no,
                                  amaun = (Double)cmpds.amt_cmpd,
                                  status_bayaran = status.status_name,
                                  akta_kesalahan = acts.act_name,
                                  kod_kesalahan = off.offense_code,
                                  kod_seksyen = sec.section_code,
                                  kod_uuk = uuk.uuk_code,
                                  arahan = cmpds.instruction,
                                  lokasi_kesalahan = cmpds.offs_location,
                                  cara_serahan = del.deliver_name,
                                  TarikhData = cmpds.created_at,
                                  //nama_pegawai = user.full_name,
                                  
                                  
                              }
                             ).ToList();
                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
        public async Task<IActionResult> GeneratePDF(int CompoundID)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                var compound = await _tenantDBContext.trn_cmpds.AsNoTracking().FirstOrDefaultAsync(x => x.trn_cmpd_id == CompoundID);
                var proofs = await _tenantDBContext.trn_cmpd_imgs.Where(x => x.trn_cmpd_id == compound.trn_cmpd_id).AsNoTracking().ToListAsync();

                compound = await GeneratePdfTicket(compound, proofs);
                _tenantDBContext.trn_cmpds.Update(compound);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(compound, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta pdf")));
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

        private async Task<trn_cmpd> GeneratePdfTicket(trn_cmpd record, List<trn_cmpd_img>? proofs)
        {
            trn_cmpd result = record;
            try
            {
                var tenantInfo = await GetTenantInfos();
                var UploadPath = await getUploadPath(record);
                string Filename = $"{GetValidFilename(record.cmpd_ref_no)}.pdf";
                var Fullpath = Path.Combine(UploadPath, Filename);
                string Pathurl = await getViewUrl(record);
                #region Massage Data
                var initQuery = _tenantDBContext.trn_cmpds
                                .Where(t => t.trn_cmpd_id == record.trn_cmpd_id)
                                .GroupJoin(
                                    _tenantDBContext.ref_delivers,
                                    t => t.deliver_id,
                                    d => d.deliver_id,
                                    (t, gd) => new { trn_cmpd = t, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, rdel) => new { jd.trn_cmpd, ref_deliver = rdel, mst_licensee = (mst_licensee)null, mst_taxtholder = (mst_taxholder)null, mst_owner = (mst_owner)null }
                                );

                if (record.is_tax == true)
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_taxholders,
                                    t => t.trn_cmpd.tax_accno,
                                    d => d.tax_accno,
                                    (t, gd) => new { t.trn_cmpd, t.ref_deliver, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mth) => new { jd.trn_cmpd, jd.ref_deliver, mst_licensee = (mst_licensee)null, mst_taxtholder = mth }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_premis,
                                    t => t.mst_taxtholder.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_cmpd, t.ref_deliver, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_cmpd, jd.ref_deliver, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
                                );
                }
                else
                {
                    initQuery = initQuery
                                .GroupJoin(
                                    _tenantDBContext.mst_licensees,
                                    t => t.trn_cmpd.license_id,
                                    d => d.licensee_id,
                                    (t, gd) => new { t.trn_cmpd, t.ref_deliver, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mli) => new { jd.trn_cmpd, jd.ref_deliver, mst_licensee = mli, mst_taxtholder = (mst_taxholder)null }
                                )
                                .GroupJoin(
                                    _tenantDBContext.mst_owner_licensees,
                                    t => t.mst_licensee.owner_icno,
                                    d => d.owner_icno,
                                    (t, gd) => new { t.trn_cmpd, t.ref_deliver, t.mst_licensee, t.mst_taxtholder, gd }
                                )
                                .SelectMany(
                                    jd => jd.gd.DefaultIfEmpty(),
                                    (jd, mto) => new { jd.trn_cmpd, jd.ref_deliver, jd.mst_licensee, jd.mst_taxtholder, mst_owner = (mst_owner)mto }
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

                //var mapImage = (record.cmpd_latitude.HasValue && record.cmpd_longitude.HasValue) ? await _mapBarcodeService.FetchGoogleMapsImageAsync(record.cmpd_latitude.Value, record.cmpd_longitude.Value) : null;
                var barcodeImage = _mapBarcodeService.GenerateBarcode(record.cmpd_ref_no, 150, 50);

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
                                    table.Cell().AlignCenter().Text($"NOTIS KESALAHAN SERTA").FontSize(4).Underline().Bold();
                                    table.Cell().AlignCenter().PaddingBottom(5).Text($"TAWARAN KOMPAUN").FontSize(4).Underline().Bold();

                                });

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
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

                                        table.Cell().PaddingLeft(5).AlignLeft().Text("No Kompaun :");
                                        table.Cell().AlignRight().Text($"{record.cmpd_ref_no}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Tarikh & Masa :");
                                        table.Cell().AlignRight().Text($"{record.created_at?.ToString("dd/MM/yyyy hh:mm:ss tt")}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Akta Kesalahan:");
                                        table.Cell().AlignRight().Text($"{aktaKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Kod Kesalahan :");
                                        table.Cell().AlignRight().Text($"{kodKesalahan}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Butir-Butir Kesalahan :");
                                        table.Cell().AlignRight().Text($"{ticketASUO.ref_law_offense.offense_description}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Cara Penyerahan :");
                                        table.Cell().AlignRight().Text($"{ticketDet.ref_deliver.deliver_name}");
                                        table.Cell().PaddingLeft(5).AlignLeft().Text("Arahan :");
                                        table.Cell().AlignRight().Text($"{record.instruction}");
                                    });

                                    string compoundAmt = "";
                                    if (record.amt_cmpd.HasValue)
                                    {
                                        compoundAmt = string.Format("{0:C}", record.amt_cmpd);
                                    }

                                    table.Cell().PaddingBottom(5).Text(text =>
                                    {
                                        text.Justify();
                                        text.Span($"Saya bersedia mengkompaun kesalahan ini dengan Kadar Kompaun {compoundAmt}. Tawaran ini berkuat kuasa dalam tempoh 14 hari dari tarikh notis ini. Kegagalan menjelaskan bayaran kompaun akan menyebabkab TINDAKAN MAHKAMAH akan diteruskan.");
                                    });


                                    if (tenantInfo?.tn_signature_byte?.Length > 0)
                                    {
                                        table.Cell().AlignLeft().PaddingLeft(5).Height(20).Image(tenantInfo.tn_signature_byte).FitArea();
                                    }
                                    else
                                    {
                                        table.Cell().AlignLeft().Height(30).Text($"   ").FontSize(4).Underline().Bold();
                                    }

                                    table.Cell().AlignLeft().Text("_________________________________");
                                    table.Cell().AlignLeft().Text("(PENGARAH UNDANG-UNDANG)");
                                    table.Cell().AlignLeft().Text("JABATAN UNDANG-UNDANG");
                                    table.Cell().AlignLeft().Text("b.p DATUK BANDAR");
                                    table.Cell().PaddingBottom(5).AlignLeft().Text($"{tenantInfo.tn_name.ToUpper()}");

                                    table.Cell().PaddingBottom(5).LineHorizontal(0.2f, Unit.Point);

                                    table.Cell().PaddingBottom(5).AlignCenter().Text("UNTUK KEGUNAAN PEJABAT");
                                    table.Cell().PaddingBottom(5).AlignCenter().AlignMiddle().MaxWidth(100).MaxHeight(15).Image(barcodeImage);
                                    table.Cell().PaddingBottom(5).AlignCenter().Text($"NO. KOMPAUN: {record.cmpd_ref_no}");

                                    table.Cell().PaddingBottom(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Border(0.2f).Column(column =>
                                        {
                                            column.Item().AlignCenter().Text("UNTUK DIISI OLEH PEGAWAI");
                                            column.Item().AlignCenter().Text("MENGKOMPAUN");
                                            column.Item().PaddingLeft(1).PaddingRight(1).LineHorizontal(0.2f, Unit.Point);
                                            column.Item().PaddingLeft(1).AlignLeft().Text("Tawaran Kompaun: RM..............");
                                            column.Item().PaddingLeft(1).AlignLeft().Text("Tarikh Tamat Kompaun: ...................");
                                            column.Item().PaddingLeft(1).AlignLeft().Text("Tandatangan & Cop pegawai");
                                            column.Item().PaddingLeft(1).PaddingBottom(10).AlignLeft().Text("pengkompaun");
                                            column.Item().PaddingLeft(1).AlignLeft().Text("Tarikh:");
                                            column.Item().PaddingLeft(1).PaddingBottom(1).AlignLeft().Text("Masa:");
                                        });

                                        table.Cell().Border(0.2f).Column(column =>
                                        {
                                            column.Item().AlignCenter().Text("UNTUK DIISI OLEH");
                                            column.Item().AlignCenter().Text("ORANG KENA KOMPAUN");
                                            column.Item().PaddingLeft(1).PaddingRight(1).LineHorizontal(0.2f, Unit.Point);
                                            column.Item().PaddingLeft(1).PaddingBottom(5).AlignLeft().Text($"Saya menerima tawaran mengkompaun suatu kesalahan bernombor {record.cmpd_ref_no}");

                                            column.Item().PaddingLeft(1).AlignLeft().Text("Nama:");
                                            column.Item().PaddingLeft(1).AlignLeft().Text("No. Kad Pengenalan:");
                                            column.Item().PaddingLeft(1).PaddingBottom(5).AlignLeft().Text("Alamat:");
                                        });
                                    });

                                    table.Cell().PaddingBottom(5).Text(text =>
                                    {
                                        text.AlignCenter();
                                        text.Span("* RESIT INI DIAKUI SAH SETELAH DICITAK OLEH MESIN PENCETAK RESIT MBDK *").FontSize(2.7f);
                                    });
                                });

                                //col.Item().Text("Google Maps Image:");
                                ////col.Item().Image(mapImage);
                                //col.Item().Text("Generated Barcode:");
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

        private async Task<string?> getUploadPath(trn_cmpd? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");

            result = await getBaseUploadPath("compound", stringDate);

            return result;
        }

        private async Task<string?> getViewUrl(trn_cmpd? record)
        {
            string? result;
            string stringDate = (record?.created_at ?? DateTime.Now).ToString("yyyyMMdd");
            result = await getBaseViewUrl("compound", stringDate);
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
