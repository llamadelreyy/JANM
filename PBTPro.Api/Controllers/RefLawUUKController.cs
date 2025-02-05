/*
Project: PBT Pro
Description: RefLawOffense controller
Author: Ismail
Date: January 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
03/01/2025 - initial create
*/
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Globalization;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RefLawUUKController : IBaseController
    {
        private readonly PBTProBkgdSM _bkgdSM;
        private readonly ILogger<RefLawUUKController> _logger;
        private readonly string _feature = "REF_LAW_UUK";
        private long _maxFileSize = 15 * 1024 * 1024;
        private List<string> _allowedFileExtensions = new List<string> { ".csv" };

        public RefLawUUKController(PBTProDbContext dbContext, ILogger<RefLawUUKController> logger, PBTProBkgdSM bkgdSM) : base(dbContext)
        {
            _logger = logger;
            _bkgdSM = bkgdSM;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var ref_law_uuks = await _dbContext.ref_law_uuks.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_uuks.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_uuks, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetListByAct(string act_code)
        {
            try
            {
                var ref_law_uuks = await _dbContext.ref_law_uuks.Where(x => x.act_code.ToUpper() == act_code.ToUpper() && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_uuks.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_uuks, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetListBySection(string section_code)
        {
            try
            {
                var ref_law_uuks = await _dbContext.ref_law_uuks.Where(x => x.section_code.ToUpper() == section_code.ToUpper() && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_uuks.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_uuks, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var ref_law_uuk = await _dbContext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_id == Id);

                if (ref_law_uuk == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(ref_law_uuk, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        //[Route("Create")]
        public async Task<IActionResult> Add([FromBody] ref_law_uuk InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.uuk_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.uuk_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_code.ToUpper() == InputModel.uuk_name.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "OFFENSE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Kesalalah telah wujud")));
                }
                #endregion

                ref_law_uuk ref_law_uuk = new ref_law_uuk
                {
                    act_code = InputModel.act_code,
                    section_code = InputModel.section_code,
                    uuk_code = InputModel.uuk_code,
                    uuk_name = InputModel.uuk_name,
                    uuk_description = InputModel.uuk_description,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.ref_law_uuks.Add(ref_law_uuk);
                await _dbContext.SaveChangesAsync();

                return Ok(ref_law_uuk, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah undang-undang kecil (UUK)")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_law_uuk InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var ref_law_uuk = await _dbContext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_id == Id);
                if (ref_law_uuk == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.uuk_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.uuk_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (ref_law_uuk.uuk_code != InputModel.uuk_code)
                {
                    var isExists = await _dbContext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_code.ToUpper() == InputModel.uuk_name.ToUpper());
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "OFFENSE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Kesalalah telah wujud")));
                    }
                }
                #endregion

                ref_law_uuk.act_code = InputModel.act_code;
                ref_law_uuk.section_code = InputModel.section_code;
                ref_law_uuk.uuk_code = InputModel.uuk_code;
                ref_law_uuk.uuk_name = InputModel.uuk_name;
                ref_law_uuk.uuk_description = InputModel.uuk_description;
                ref_law_uuk.modifier_id = runUserID;
                ref_law_uuk.modified_at = DateTime.Now;

                _dbContext.ref_law_uuks.Update(ref_law_uuk);
                await _dbContext.SaveChangesAsync();

                return Ok(ref_law_uuk, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai undang-undang kecil (UUK)")));
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
                #region Validation
                var ref_law_uuk = await _dbContext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_id == Id);
                if (ref_law_uuk == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ref_law_uuks.Remove(ref_law_uuk);
                await _dbContext.SaveChangesAsync();

                return Ok(ref_law_uuk, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang undang-undang kecil (UUK)")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromForm] upload_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (InputModel.file == null || InputModel.file.Length == 0)
                {
                    return Error("", SystemMesg(_feature, "FILE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Fail diperlukan")));
                }

                if (!IsFileExtensionAllowed(InputModel.file, _allowedFileExtensions))
                {
                    var imageFileExtString = String.Join(", ", _allowedFileExtensions.ToList());
                    List<string> param = new List<string> { imageFileExtString };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong [0]."), param));
                }

                if (!IsFileSizeWithinLimit(InputModel.file, _maxFileSize))
                {
                    List<string> param = new List<string> { FormatFileSize(_maxFileSize) };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                }

                string ServiceName = "RefLawUUKImport";
                var isExists = _bkgdSM.GetBackgroundServiceQueue(ServiceName);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "IMPORT_BGTASK_EXISTS", MessageTypeEnum.Error, string.Format("Terdapat proses import masuk sedang dilaksanakan, sila cuba lagi setelah proses tersebut selesai.")));
                }
                #endregion

                using (var reader = new StreamReader(InputModel.file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var expectedHeaders = typeof(importRefLawUUK).GetProperties()
                        .Select(p => p.Name)
                        .ToList();
                    var actualHeaders = csv.HeaderRecord.ToList();
                    if (!expectedHeaders.SequenceEqual(actualHeaders, StringComparer.OrdinalIgnoreCase))
                    {
                        return Error("", SystemMesg(_feature, "INVALID_HEADER", MessageTypeEnum.Error, string.Format("Lajur pengepala fail tidak sah.")));
                    }

                    // Read all records into a list before starting the background task
                    var records = csv.GetRecords<importRefLawUUK>().ToList();

                    _bkgdSM.StartBackgroundService(ServiceName);
                    _bkgdSM.EnqueueWorkItem(ServiceName, async token =>
                    {
                        try
                        {
                            using (PBTProDbContext _dbcontext = new PBTProDbContext())
                            {
                                //var records = csv.GetRecords<importRefLawAct>().ToList();
                                List<ref_law_uuk> NewRecs = new List<ref_law_uuk>();
                                List<ref_law_uuk> UpdRecs = new List<ref_law_uuk>();

                                //while (csv.Read()) <-- read line by line, style 1
                                foreach (var record in records)
                                {
                                    bool isNew = true;
                                    //var record = csv.GetRecord<importRefLawAct>(); <-- read line by line, style 1

                                    #region Sigle Validation
                                    if (string.IsNullOrWhiteSpace(record.uuk_code) || string.IsNullOrWhiteSpace(record.uuk_name))
                                    {
                                        continue;
                                    }

                                    if (NewRecs.Any(r => r.uuk_code.ToUpper() == record.uuk_code.ToUpper()) || UpdRecs.Any(r => r.uuk_code.ToUpper() == record.uuk_code.ToUpper()))
                                    {
                                        continue;
                                    }

                                    if (!string.IsNullOrWhiteSpace(record.act_code))
                                    {
                                        var isActCodeExists = await _dbcontext.ref_law_acts.FirstOrDefaultAsync(x => x.act_code.ToUpper() == record.act_code.ToUpper());
                                        if (isActCodeExists == null)
                                        {
                                            continue;
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(record.section_code))
                                    {
                                        var isSectionCodeExists = await _dbcontext.ref_law_sections.FirstOrDefaultAsync(x => x.section_code.ToUpper() == record.section_code.ToUpper());
                                        if (isSectionCodeExists == null)
                                        {
                                            continue;
                                        }
                                    }
                                    #endregion

                                    var isExists = await _dbcontext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_code.ToUpper() == record.uuk_code.ToUpper());
                                    if (isExists != null)
                                    {
                                        isExists.act_code = record.act_code;
                                        isExists.section_code = record.section_code;
                                        isExists.uuk_name = record.uuk_name;
                                        isExists.uuk_description = record.uuk_description;
                                        isExists.modified_at = DateTime.Now;
                                        isExists.modifier_id = runUserID;
                                        UpdRecs.Add(isExists);
                                    }
                                    else
                                    {
                                        ref_law_uuk newRec = new ref_law_uuk
                                        {
                                            act_code = record.act_code,
                                            section_code = record.section_code,
                                            uuk_code = record.uuk_code,
                                            uuk_name = record.uuk_name,
                                            uuk_description = record.uuk_description,
                                            created_at = DateTime.Now,
                                            creator_id = runUserID
                                        };

                                        NewRecs.Add(newRec);
                                    }

                                    if ((NewRecs.Count + UpdRecs.Count) >= 200)
                                    {
                                        await SaveImportsToDatabase(NewRecs, UpdRecs);
                                        NewRecs.Clear();
                                        UpdRecs.Clear();
                                    }
                                }

                                if ((NewRecs.Count + UpdRecs.Count) > 0)
                                {
                                    await SaveImportsToDatabase(NewRecs, UpdRecs);
                                }
                            }
                        }
                        catch (OperationCanceledException OCex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, OCex.Message, OCex.InnerException));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        }


                        try
                        {
                            _bkgdSM.RemoveBackgroundService(ServiceName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        }
                    });
                }

                return Ok("", SystemMesg(_feature, "IMPORT_QUEUE", MessageTypeEnum.Success, string.Format("Berjaya memuat naik fail, proces import rekot akan dilakukan dibelakang tabir")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Private Logic
        private class importRefLawUUK
        {
            public string? act_code { get; set; }
            public string? section_code { get; set; }
            public string uuk_code { get; set; }
            public string uuk_name { get; set; }
            public string? uuk_description { get; set; }
        }

        private async Task SaveImportsToDatabase(List<ref_law_uuk> newRecs, List<ref_law_uuk> updRecs)
        {
            using (PBTProDbContext _dbcontext2 = new PBTProDbContext())
            {
                try
                {
                    if (newRecs.Count > 0)
                    {
                        _dbcontext2.ref_law_uuks.AddRange(newRecs);
                    }
                    if (updRecs.Count > 0)
                    {
                        _dbcontext2.ref_law_uuks.UpdateRange(updRecs);
                    }
                    await _dbcontext2.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }
            }
        }
        #endregion
    }
}
