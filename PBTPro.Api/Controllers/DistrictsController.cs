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
    public class DistrictsController : IBaseController
    {
        private readonly ILogger<DistrictsController> _logger;
        private readonly string _feature = "MST_DISTRICTS";

        public DistrictsController(PBTProDbContext dbContext, ILogger<DistrictsController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var mst_districts = await _dbContext.mst_districts.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (mst_districts.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_districts, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{StateCode}")]
        public async Task<IActionResult> GetListByState(string StateCode)
        {
            try
            {
                var mst_districts = await _dbContext.mst_districts.Where(x => x.state_code == StateCode && x.is_deleted != true).AsNoTracking().ToListAsync();

                if (mst_districts.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_districts, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var district = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_id == Id);

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

        [HttpGet("{Code}")]
        public async Task<IActionResult> GetDetailByCode(string Code)
        {
            try
            {
                var district = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_code == Code);

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
        public async Task<IActionResult> Add([FromBody] mst_district InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.district_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.district_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_code.ToUpper() == InputModel.district_code.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "DISTRICT_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Daerah telah wujud")));
                }
                #endregion

                mst_district district = new mst_district
                {
                    state_code = InputModel.state_code,
                    district_code = InputModel.district_code,
                    district_name = InputModel.district_name,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.mst_districts.Add(district);
                await _dbContext.SaveChangesAsync();

                return Ok(district, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah daerah")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] mst_district InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var district = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_id == Id);
                if (district == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.district_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.district_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (district.district_code != InputModel.district_code)
                {
                    var isExists = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_code.ToUpper() == InputModel.district_code.ToUpper() && x.district_id != district.district_id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "DISTRICT_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Daerah telah wujud")));
                    }
                }
                #endregion
                district.state_code = InputModel.state_code;
                district.district_code = InputModel.district_code;
                district.district_name = InputModel.district_name;
                district.modifier_id = runUserID;
                district.modified_at = DateTime.Now;

                _dbContext.mst_districts.Update(district);
                await _dbContext.SaveChangesAsync();

                return Ok(district, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai daerah")));
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
                var district = await _dbContext.mst_districts.FirstOrDefaultAsync(x => x.district_id == Id);
                if (district == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.mst_districts.Remove(district);
                await _dbContext.SaveChangesAsync();

                return Ok(district, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang daerah")));
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


