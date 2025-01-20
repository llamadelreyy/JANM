/*
Project: PBT Pro
Description: States controller
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
    public class StatesController : IBaseController
    {
        private readonly ILogger<StatesController> _logger;
        private readonly string _feature = "MST_STATES";

        public StatesController(PBTProDbContext dbContext, ILogger<StatesController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var mst_states = await _dbContext.mst_states.Where(x => x.is_deleted != true).Join(
                    _dbContext.mst_countries,
                    state => state.country_code,
                    country => country.country_code,
                    (state, country) => new mst_state {
                        state_id = state.state_id,
                        state_code = state.state_code,
                        state_name = state.state_name,
                        country_code = state.country_code,
                        is_deleted = state.is_deleted,
                        creator_id = state.creator_id,
                        created_at = state.created_at,
                        modifier_id = state.modifier_id,
                        modified_at = state.modified_at,
                        //virtual field
                        country_name = country.country_name
                    }
                ).AsNoTracking().ToListAsync();

                if (mst_states.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_states, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{CountryCode}")]
        public async Task<IActionResult> GetListByCountry(string CountryCode)
        {
            try
            {
                var mst_states = await _dbContext.mst_states.Where(x => x.country_code == CountryCode && x.is_deleted != true).Join(
                    _dbContext.mst_countries,
                    state => state.country_code,
                    country => country.country_code,
                    (state, country) => new mst_state
                    {
                        state_id = state.state_id,
                        state_code = state.state_code,
                        state_name = state.state_name,
                        country_code = state.country_code,
                        is_deleted = state.is_deleted,
                        creator_id = state.creator_id,
                        created_at = state.created_at,
                        modifier_id = state.modifier_id,
                        modified_at = state.modified_at,
                        //virtual field
                        country_name = country.country_name
                    }).AsNoTracking().ToListAsync();

                if (mst_states.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(mst_states, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                var state = await _dbContext.mst_states.Join(
                    _dbContext.mst_countries,
                    state => state.country_code,
                    country => country.country_code,
                    (state, country) => new mst_state
                    {
                        state_id = state.state_id,
                        state_code = state.state_code,
                        state_name = state.state_name,
                        country_code = state.country_code,
                        is_deleted = state.is_deleted,
                        creator_id = state.creator_id,
                        created_at = state.created_at,
                        modifier_id = state.modifier_id,
                        modified_at = state.modified_at,
                        //virtual field
                        country_name = country.country_name
                    }).FirstOrDefaultAsync(x => x.state_id == Id);

                if (state == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(state, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
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
                var state = await _dbContext.mst_states.Join(
                    _dbContext.mst_countries,
                    state => state.country_code,
                    country => country.country_code,
                    (state, country) => new mst_state
                    {
                        state_id = state.state_id,
                        state_code = state.state_code,
                        state_name = state.state_name,
                        country_code = state.country_code,
                        is_deleted = state.is_deleted,
                        creator_id = state.creator_id,
                        created_at = state.created_at,
                        modifier_id = state.modifier_id,
                        modified_at = state.modified_at,
                        //virtual field
                        country_name = country.country_name
                    }).FirstOrDefaultAsync(x => x.state_code == Code);

                if (state == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(state, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] mst_state InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.state_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.state_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                var isExists = await _dbContext.mst_states.FirstOrDefaultAsync(x => x.state_code.ToUpper() == InputModel.state_code.ToUpper());
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "STATE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Negeri telah wujud")));
                }
                #endregion

                mst_state state = new mst_state
                {
                    country_code = InputModel.country_code,
                    state_code = InputModel.state_code,
                    state_name = InputModel.state_name,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.mst_states.Add(state);
                await _dbContext.SaveChangesAsync();

                return Ok(state, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah negeri")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] mst_state InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var state = await _dbContext.mst_states.FirstOrDefaultAsync(x => x.state_id == Id);
                if (state == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.state_code))
                {
                    return Error("", SystemMesg(_feature, "CODE_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Kod diperlukan")));
                }

                if (string.IsNullOrEmpty(InputModel.state_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Nama diperlukan")));
                }

                if (state.state_code != InputModel.state_code)
                {
                    var isExists = await _dbContext.mst_states.FirstOrDefaultAsync(x => x.state_code.ToUpper() == InputModel.state_code.ToUpper() && x.state_id != state.state_id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "STATE_CODE_ISEXISTS", MessageTypeEnum.Error, string.Format("Kod Negeri telah wujud")));
                    }
                }
                #endregion
                state.country_code = InputModel.country_code;
                state.state_code = InputModel.state_code;
                state.state_name = InputModel.state_name;
                state.modifier_id = runUserID;
                state.modified_at = DateTime.Now;

                _dbContext.mst_states.Update(state);
                await _dbContext.SaveChangesAsync();

                return Ok(state, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai negeri")));
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
                var state = await _dbContext.mst_states.FirstOrDefaultAsync(x => x.state_id == Id);
                if (state == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _dbContext.mst_states.Remove(state);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    state.is_deleted = true;
                    state.modifier_id = runUserID;
                    state.modified_at = DateTime.Now;

                    _dbContext.mst_states.Update(state);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(state, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang negeri")));
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


