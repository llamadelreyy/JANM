/*
Project: PBT Pro
Description: District controller
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
    public class TownsController : IBaseController
    {
        private readonly ILogger<TownsController> _logger;
        private readonly string _feature = "MST_TOWNS";

        public TownsController(PBTProDbContext dbContext, ILogger<TownsController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var mst_towns = await _dbContext.mst_towns.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (mst_towns.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_towns, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{DistrictCode}")]
        public async Task<IActionResult> GetListByDistrict(string DistrictCode)
        {
            try
            {
                var mst_towns = await _dbContext.mst_towns.Where(x => x.district_code == DistrictCode && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (mst_towns.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_towns, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var town = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.town_id == Id);

                if (town == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(town, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
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
                var district = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.district_code == Code);

                if (district == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(district, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] mst_town InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.town_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.town_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.town_code.ToUpper() == InputModel.town_code.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "TOWN_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Bandar telah wujud")));
                }
                #endregion

                mst_town town = new mst_town
                {
                    district_code = InputModel.district_code,
                    town_code = InputModel.town_code,
                    town_name = InputModel.town_name,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.mst_towns.Add(town);
                await _dbContext.SaveChangesAsync();

                return Ok(town, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah bandar")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] mst_town InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var town = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.town_id == Id);
                if (town == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.town_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.town_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (town.town_code != InputModel.town_code)
                {
                    var isExists = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.district_code.ToUpper() == InputModel.district_code.ToUpper() && x.town_id != town.town_id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "TOWN_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Bander telah wujud")));
                    }
                }
                #endregion
                town.district_code = InputModel.district_code;
                town.town_code = InputModel.town_code;
                town.town_name = InputModel.town_name;
                town.modifier_id = runUserID;
                town.modified_at = DateTime.Now;

                _dbContext.mst_towns.Update(town);
                await _dbContext.SaveChangesAsync();

                return Ok(town, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai bandar")));
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
                var district = await _dbContext.mst_towns.FirstOrDefaultAsync(x => x.town_id == Id);
                if (district == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.mst_towns.Remove(district);
                await _dbContext.SaveChangesAsync();

                return Ok(district, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang bandar")));
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


