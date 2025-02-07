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
    public class RefLawOffenseController : IBaseController
    {
        private readonly PBTProBkgdSM _bkgdSM;
        private readonly ILogger<RefLawOffenseController> _logger;
        private readonly string _feature = "REF_LAW_OFFENSE";
        private long _maxFileSize = 15 * 1024 * 1024;
        private List<string> _allowedFileExtensions = new List<string> { ".csv" };

        public RefLawOffenseController(PBTProDbContext dbContext, ILogger<RefLawOffenseController> logger, PBTProBkgdSM bkgdSM) : base(dbContext)
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
                var ref_law_offenses = await _dbContext.ref_law_offenses.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_offenses.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_offenses, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var ref_law_offenses = await _dbContext.ref_law_offenses.Where(x => x.act_code.ToUpper() == act_code.ToUpper() && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_offenses.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_offenses, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var ref_law_offenses = await _dbContext.ref_law_offenses.Where(x => x.section_code.ToUpper() == section_code.ToUpper() && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_offenses.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_offenses, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetListByUUK(string uuk_code)
        {
            try
            {
                var ref_law_offenses = await _dbContext.ref_law_offenses.Where(x => x.uuk_code.ToUpper() == uuk_code.ToUpper() && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (ref_law_offenses.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(ref_law_offenses, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var ref_law_offense = await _dbContext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_id == Id);

                if (ref_law_offense == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(ref_law_offense, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        //[Route("Create")]
        public async Task<IActionResult> Add([FromBody] ref_law_offense InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.offense_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.offense_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_code.ToUpper() == InputModel.offense_code.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "OFFENSE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Kesalalah telah wujud")));
                }
                #endregion

                ref_law_offense ref_law_offense = new ref_law_offense
                {
                    act_code = InputModel.act_code,
                    section_code = InputModel.section_code,
                    uuk_code = InputModel.uuk_code,
                    offense_code = InputModel.offense_code,
                    offense_name = InputModel.offense_name,
                    offense_description = InputModel.offense_description,
                    offense_content = InputModel.offense_content,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.ref_law_offenses.Add(ref_law_offense);
                await _dbContext.SaveChangesAsync();

                return Ok(ref_law_offense, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah kesalahan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_law_offense InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var ref_law_offense = await _dbContext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_id == Id);
                if (ref_law_offense == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.offense_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.offense_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (ref_law_offense.offense_code != InputModel.offense_code)
                {
                    var isExists = await _dbContext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_code.ToUpper() == InputModel.offense_code.ToUpper() && x.offense_id != ref_law_offense.offense_id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "OFFENSE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Kesalalah telah wujud")));
                    }
                }
                #endregion

                ref_law_offense.act_code = InputModel.act_code;
                ref_law_offense.section_code = InputModel.section_code;
                ref_law_offense.uuk_code = InputModel.uuk_code;
                ref_law_offense.offense_code = InputModel.offense_code;
                ref_law_offense.offense_name = InputModel.offense_name;
                ref_law_offense.offense_description = InputModel.offense_description;
                ref_law_offense.offense_content = InputModel.offense_content;
                ref_law_offense.modifier_id = runUserID;
                ref_law_offense.modified_at = DateTime.Now;

                _dbContext.ref_law_offenses.Update(ref_law_offense);
                await _dbContext.SaveChangesAsync();

                return Ok(ref_law_offense, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai kesalahan")));
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
                var ref_law_offense = await _dbContext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_id == Id);
                if (ref_law_offense == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _dbContext.ref_law_offenses.Remove(ref_law_offense);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ref_law_offense.is_deleted = true;
                    ref_law_offense.modifier_id = runUserID;
                    ref_law_offense.modified_at = DateTime.Now;

                    _dbContext.ref_law_offenses.Update(ref_law_offense);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(ref_law_offense, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang kesalahan")));
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

                string ServiceName = "RefLawOffenseImport";
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
                    var expectedHeaders = typeof(importRefLawOffense).GetProperties()
                        .Select(p => p.Name)
                        .ToList();
                    var actualHeaders = csv.HeaderRecord.ToList();
                    if (!expectedHeaders.SequenceEqual(actualHeaders, StringComparer.OrdinalIgnoreCase))
                    {
                        return Error("", SystemMesg(_feature, "INVALID_HEADER", MessageTypeEnum.Error, string.Format("Lajur pengepala fail tidak sah.")));
                    }

                    // Read all records into a list before starting the background task
                    var records = csv.GetRecords<importRefLawOffense>().ToList();

                    _bkgdSM.StartBackgroundService(ServiceName);
                    _bkgdSM.EnqueueWorkItem(ServiceName, async token =>
                    {
                        try
                        {
                            using (PBTProDbContext _dbcontext = new PBTProDbContext())
                            {
                                //var records = csv.GetRecords<importRefLawAct>().ToList();
                                List<ref_law_offense> NewRecs = new List<ref_law_offense>();
                                List<ref_law_offense> UpdRecs = new List<ref_law_offense>();

                                //while (csv.Read()) <-- read line by line, style 1
                                foreach (var record in records)
                                {
                                    bool isNew = true;
                                    //var record = csv.GetRecord<importRefLawAct>(); <-- read line by line, style 1

                                    #region Sigle Validation
                                    if (string.IsNullOrWhiteSpace(record.offense_code) || string.IsNullOrWhiteSpace(record.offense_name))
                                    {
                                        continue;
                                    }

                                    if (NewRecs.Any(r => r.offense_code.ToUpper() == record.offense_code.ToUpper()) || UpdRecs.Any(r => r.offense_code.ToUpper() == record.offense_code.ToUpper()))
                                    {
                                        continue;
                                    }

                                    if (!string.IsNullOrWhiteSpace(record.act_code))
                                    {
                                        var isActCodeExists = await _dbcontext.ref_law_acts.FirstOrDefaultAsync(x => x.act_code.ToUpper() == record.act_code.ToUpper());
                                        if(isActCodeExists == null)
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

                                    if (!string.IsNullOrWhiteSpace(record.uuk_code))
                                    {
                                        var isUUKCodeExists = await _dbcontext.ref_law_uuks.FirstOrDefaultAsync(x => x.uuk_code.ToUpper() == record.uuk_code.ToUpper());
                                        if (isUUKCodeExists == null)
                                        {
                                            continue;
                                        }
                                    }
                                    #endregion

                                    var isExists = await _dbcontext.ref_law_offenses.FirstOrDefaultAsync(x => x.offense_code.ToUpper() == record.offense_code.ToUpper());
                                    if (isExists != null)
                                    {
                                        isExists.act_code = record.act_code;
                                        isExists.section_code = record.section_code;
                                        isExists.uuk_code = record.section_code;
                                        isExists.offense_name = record.offense_name;
                                        isExists.offense_description = record.offense_description;
                                        isExists.offense_content = record.offense_content;
                                        isExists.modified_at = DateTime.Now;
                                        isExists.modifier_id = runUserID;
                                        UpdRecs.Add(isExists);
                                    }
                                    else
                                    {
                                        ref_law_offense newRec = new ref_law_offense
                                        {
                                            act_code = record.act_code,
                                            section_code = record.section_code,
                                            uuk_code = record.uuk_code,
                                            offense_code = record.uuk_code,
                                            offense_name = record.offense_name,
                                            offense_description = record.offense_description,
                                            offense_content = record.offense_content,
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
        private class importRefLawOffense
        {
            public string? act_code { get; set; }
            public string? section_code { get; set; }
            public string? uuk_code { get; set; }
            public string offense_code { get; set; }
            public string offense_name { get; set; }
            public string? offense_description { get; set; }
            public string? offense_content { get; set; }
        }

        private async Task SaveImportsToDatabase(List<ref_law_offense> newRecs, List<ref_law_offense> updRecs)
        {
            using (PBTProDbContext _dbcontext2 = new PBTProDbContext())
            {
                try
                {
                    if (newRecs.Count > 0)
                    {
                        _dbcontext2.ref_law_offenses.AddRange(newRecs);
                    }
                    if (updRecs.Count > 0)
                    {
                        _dbcontext2.ref_law_offenses.UpdateRange(updRecs);
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
