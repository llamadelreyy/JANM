/*
Project: PBT Pro
Description: Countries controller
Author: Ismail
Date: January 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
14/01/2025 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CountriesController : IBaseController
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly string _feature = "MST_COUNTRIES";

        public CountriesController(PBTProDbContext dbContext, ILogger<CountriesController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var mst_countries = await _dbContext.mst_countries.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (mst_countries.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_countries, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var country = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_id == Id);

                if (country == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(country, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Code}")]
        public async Task<IActionResult> GetDetailByCode(string Code)
        {
            try
            {
                var country = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_code == Code);

                if (country == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(country, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] mst_country InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.country_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.country_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_code.ToUpper() == InputModel.country_code.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "COUNTRY_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Negara telah wujud")));
                }
                #endregion

                mst_country country = new mst_country
                {
                    country_code = InputModel.country_code,
                    country_name = InputModel.country_name,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.mst_countries.Add(country);
                await _dbContext.SaveChangesAsync();

                return Ok(country, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah negara")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] mst_country InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var country = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_id == Id);
                if (country == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.country_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.country_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (country.country_code != InputModel.country_code)
                {
                    var isExists = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_code.ToUpper() == InputModel.country_code.ToUpper() && x.country_id != country.country_id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "COUNTRY_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Negara telah wujud")));
                    }
                }
                #endregion

                country.country_code = InputModel.country_code;
                country.country_name = InputModel.country_name;
                country.modifier_id = runUserID;
                country.modified_at = DateTime.Now;

                _dbContext.mst_countries.Update(country);
                await _dbContext.SaveChangesAsync();

                return Ok(country, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai negara")));
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
                var country = await _dbContext.mst_countries.FirstOrDefaultAsync(x => x.country_id == Id);
                if (country == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _dbContext.mst_countries.Remove(country);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    country.is_deleted = true;
                    country.modifier_id = runUserID;
                    country.modified_at = DateTime.Now;

                    _dbContext.mst_countries.Update(country);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(country, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang negara")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Private Logic
        #endregion
    }
}


