/*
Project: PBT Pro
Description: patrol api, to be used when mobile stop / start patrolling
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- this API using 3rd party SignalR broadcast to sent message to user for updating patrolling status

Changes Logs:
06/11/2024 - initial create
20/2/2024 - add user_id field for each method. Update method GetScheduleByUserId to list and filter by current date
*/
using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Utils.Filtering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Reactive.Concurrency;
using System.Reflection;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ScheduleController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly ILogger<ScheduleController> _logger;

        private readonly string _feature = "SCHEDULE";

        public ScheduleController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<ScheduleController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetList(string? crs = null)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                var data = await _tenantDBContext.mst_patrol_schedules.Where(x => x.is_deleted == false).Select(x => new
                {
                    schedule_id = x.schedule_id,
                    idno = x.idno,
                    start_time = x.start_time,
                    end_time = x.end_time,
                    status_id = x.status_id,
                    is_scheduled = x.is_scheduled,
                    loc_name = x.loc_name,
                    type_id = x.type_id,
                    dept_id = x.dept_id,
                    cnt_cmpd = x.cnt_cmpd,
                    cnt_notice = x.cnt_notice,
                    cnt_notes = x.cnt_notes,
                    cnt_seizure = x.cnt_seizure,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                    modifier_id = runUserID,
                    modified_at = DateTime.Now,
                    is_deleted = false,
                    start_location = x.start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.start_location))
                                    : null,

                    end_location = x.end_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.end_location))
                                    : null,
                    district_code = x.district_code,
                    town_code = x.town_code,
                    user_id = x.user_id,

                }).AsNoTracking().ToListAsync();

                var result = (
                        from schedule in data
                        join user in _dbContext.Users on schedule.idno equals user.IdNo
                        join department in _tenantDBContext.ref_departments on user.dept_id equals department.dept_id
                        join seksyen in _tenantDBContext.ref_divisions on user.div_id equals seksyen.div_id
                        join unit in _tenantDBContext.ref_units on user.unit_id equals unit.unit_id
                        join daerah in _dbContext.mst_districts on schedule.district_code equals daerah.district_code
                        join bandar in _dbContext.mst_towns on schedule.town_code equals bandar.town_code
                        join type in _tenantDBContext.ref_patrol_types on schedule.type_id equals type.type_id

                        // Group by `schedule_id` to avoid duplicates
                        group new { schedule, user, department, seksyen, unit, daerah, bandar, type } by schedule.schedule_id into grouped
                        select new PatrolViewModel
                        {
                            scheduleId = grouped.First().schedule.schedule_id,
                            OfficerName = grouped.First().user.full_name,
                            CreatedAt = grouped.First().schedule.created_at,
                            DeptName = grouped.First().department.dept_name,
                            SectionName = grouped.First().seksyen.div_name,
                            UnitName = grouped.First().unit.unit_name,
                            StartTime = grouped.First().schedule.start_time,
                            EndTime = grouped.First().schedule.end_time,
                            DistrictCode = grouped.First().daerah.district_code,
                            DistrictName = grouped.First().daerah.district_name,
                            TownCode = grouped.First().bandar.town_code,
                            TownName = grouped.First().bandar.town_name,
                            PatrolStatus = Convert.ToString(grouped.First().schedule.status_id),
                            TypeId = grouped.First().type.type_id,
                            TypeName = grouped.First().type.type_name,
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PatrolViewModel InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                if (InputModel.StartTime <= DateTime.Today || InputModel.EndTime <= DateTime.Today)
                {
                    return Error("", SystemMesg(_feature, "PATROL_START_END_TIME", MessageTypeEnum.Error, string.Format("Tarikh hari sebelum tidak dibenarkan.")));
                }

                var existingSchedules = await _tenantDBContext.mst_patrol_schedules
                                .Where(s => s.idno == InputModel.ICNo && s.is_deleted == false)
                                .ToListAsync();

                foreach (var schedule in existingSchedules)
                {
                    if (InputModel.StartTime >= schedule.start_time && InputModel.EndTime <= schedule.end_time)
                    {
                        return Error("", SystemMesg(_feature, "PATROL_TIME_WITHIN_EXISTING", MessageTypeEnum.Error, string.Format("Tarikh rondaan adalah dalam jadual yang sedia ada.")));
                    }

                    if (InputModel.StartTime < schedule.end_time && InputModel.EndTime > schedule.start_time)
                    {
                        return Error("", SystemMesg(_feature, "PATROL_OVERLAP_TIME", MessageTypeEnum.Error, string.Format("Tarikh rondaan bertindih dengan jadual lain.")));
                    }
                }

                #region store data
                mst_patrol_schedule mst_Patrol = new mst_patrol_schedule
                {
                    idno = InputModel.ICNo,
                    dept_id = InputModel.DeptId,
                    start_time = (DateTime)InputModel.StartTime,
                    end_time = (DateTime)InputModel.EndTime,
                    creator_id = runUserID,
                    created_at = InputModel.CreatedAt,
                    is_deleted = false,
                    district_code = InputModel.DistrictCode,
                    town_code = InputModel.TownCode,
                    status_id = Convert.ToInt32(InputModel.PatrolStatus),
                    user_id = runUserID,
                    is_scheduled = true,
                    type_id = InputModel.TypeId,
                };

                _tenantDBContext.mst_patrol_schedules.Add(mst_Patrol);
                await _tenantDBContext.SaveChangesAsync();

                #endregion               
                return Ok(mst_Patrol, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya tambah jadual rondaan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] PatrolViewModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.mst_patrol_schedules
                                .FirstOrDefaultAsync(x => x.schedule_id == InputModel.scheduleId);

                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.ICNo))
                {
                    return Error("", SystemMesg(_feature, "PATROL_OFFICER_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Pegawai diperlukan")));
                }

                if (InputModel.StartTime <= DateTime.Today || InputModel.EndTime <= DateTime.Today)
                {
                    return Error("", SystemMesg(_feature, "PATROL_START_END_TIME", MessageTypeEnum.Error, string.Format("Tarikh perlu hari ini dan seterusnya.")));
                }

                var existingSchedules = await _tenantDBContext.mst_patrol_schedules
                                 .Where(s => s.idno == InputModel.ICNo)
                                 .ToListAsync();

                foreach (var schedule in existingSchedules)
                {
                    bool ischange = true;
                    if (schedule.start_time == InputModel.StartTime && schedule.end_time == InputModel.EndTime)
                    {
                        ischange = false;
                    }
                    if (ischange)
                    {
                        if (InputModel.StartTime >= schedule.start_time && InputModel.EndTime <= schedule.end_time)
                        {
                            return Error("", SystemMesg(_feature, "PATROL_TIME_WITHIN_EXISTING", MessageTypeEnum.Error, string.Format("Tarikh rondaan adalah dalam jadual yang sedia ada.")));
                        }

                        if (InputModel.StartTime < schedule.end_time && InputModel.EndTime > schedule.start_time)
                        {
                            return Error("", SystemMesg(_feature, "PATROL_OVERLAP_TIME", MessageTypeEnum.Error, string.Format("Tarikh rondaan bertindih dengan jadual lain.")));
                        }
                    }
                }
                #endregion

                formField.idno = InputModel.ICNo;
                formField.dept_id = InputModel.DeptId;
                formField.start_time = (DateTime)InputModel.StartTime;
                formField.end_time = (DateTime)InputModel.EndTime;
                formField.district_code = InputModel.DistrictCode;
                formField.town_code = InputModel.TownCode;
                formField.modifier_id = runUserID;
                formField.user_id = runUserID;
                formField.type_id = InputModel.TypeId;

                _tenantDBContext.mst_patrol_schedules.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListByTabType(string tabType)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                var data = await _tenantDBContext.mst_patrol_schedules.Where(x => x.is_deleted == false && x.status_id == Convert.ToInt32(tabType)).OrderBy(x => x.status_id).Select(x => new
                {
                    schedule_id = x.schedule_id,
                    idno = x.idno,
                    start_time = x.start_time,
                    end_time = x.end_time,
                    status_id = x.status_id,
                    is_scheduled = x.is_scheduled,
                    loc_name = x.loc_name,
                    type_id = x.type_id,
                    dept_id = x.dept_id,
                    cnt_cmpd = x.cnt_cmpd,
                    cnt_notice = x.cnt_notice,
                    cnt_notes = x.cnt_notes,
                    cnt_seizure = x.cnt_seizure,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                    modifier_id = runUserID,
                    modified_at = DateTime.Now,
                    is_deleted = false,
                    start_location = x.start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.start_location))
                                    : null,

                    end_location = x.end_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.end_location))
                                    : null,
                    district_code = x.district_code,
                    town_code = x.town_code,
                    user_id = x.user_id,

                }).AsNoTracking().ToListAsync();

                var result = (
                        from schedule in data
                        join user in _dbContext.Users on schedule.idno equals user.IdNo
                        join department in _tenantDBContext.ref_departments on user.dept_id equals department.dept_id
                        join seksyen in _tenantDBContext.ref_divisions on user.div_id equals seksyen.div_id
                        join unit in _tenantDBContext.ref_units on user.unit_id equals unit.unit_id
                        join daerah in _dbContext.mst_districts on schedule.district_code equals daerah.district_code
                        join bandar in _dbContext.mst_towns on schedule.town_code equals bandar.town_code
                        join type in _tenantDBContext.ref_patrol_types on schedule.type_id equals type.type_id

                        group new { schedule, user, department, seksyen, unit, daerah, bandar, type } by schedule.schedule_id into grouped
                        select new PatrolViewModel
                        {
                            scheduleId = grouped.First().schedule.schedule_id,
                            OfficerName = grouped.First().user.full_name,
                            CreatedAt = grouped.First().schedule.created_at,
                            DeptName = grouped.First().department.dept_name,
                            SectionName = grouped.First().seksyen.div_name,
                            UnitName = grouped.First().unit.unit_name,
                            StartTime = grouped.First().schedule.start_time,
                            EndTime = grouped.First().schedule.end_time,
                            DistrictCode = grouped.First().daerah.district_code,
                            DistrictName = grouped.First().daerah.district_name,
                            TownCode = grouped.First().bandar.town_code,
                            TownName = grouped.First().bandar.town_name,
                            PatrolStatus = Convert.ToString(grouped.First().schedule.status_id),
                            TypeName = grouped.First().type.type_name,
                            TypeId = grouped.First().type.type_id,
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

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.mst_patrol_schedules.Where(x => x.status_id == 3).FirstOrDefaultAsync(x => x.schedule_id == Id);

                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah. Status Belum Mula sahaja boleh dipadam.")));
                }

                #endregion

                formField.is_deleted = true;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.mst_patrol_schedules.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetScheduleByUserId(string username)
        {
            try
            {

                var data = await _tenantDBContext.mst_patrol_schedules
                    .Where(cc => cc.idno == username && cc.created_at.Date == DateTime.Today && cc.is_deleted == false)
                    .Select(x => new
                    {
                        schedule_id = x.schedule_id,
                        idno = x.idno,
                        start_time = x.start_time,
                        end_time = x.end_time,
                        status_id = x.status_id,
                        is_scheduled = x.is_scheduled,
                        loc_name = x.loc_name,
                        type_id = x.type_id,
                        dept_id = x.dept_id,
                        cnt_cmpd = x.cnt_cmpd,
                        cnt_notice = x.cnt_notice,
                        cnt_notes = x.cnt_notes,
                        cnt_seizure = x.cnt_seizure,
                        creator_id = x.creator_id,
                        created_at = x.created_at,
                        modifier_id = x.modifier_id,
                        modified_at = x.modified_at,
                        is_deleted = x.is_deleted,
                        start_location = x.start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.start_location))
                                    : null,

                        end_location = x.end_location != null
                                        ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.end_location))
                                        : null,
                        district_code = x.district_code,
                        town_code = x.town_code,
                        user_id = x.user_id,
                    })
                .ToListAsync();


                var result = (
                                from schedule in data
                                join daerah in _dbContext.mst_districts on schedule.district_code equals daerah.district_code
                                join bandar in _dbContext.mst_towns on schedule.town_code equals bandar.town_code
                                where bandar.district_code == daerah.district_code
                                select new PatrolViewModel
                                {
                                    scheduleId = schedule.schedule_id,
                                    ICNo = schedule.idno,
                                    StartTime = schedule.start_time,
                                    EndTime = schedule.end_time,
                                    PatrolId = (int)schedule.status_id,
                                    TownCode = schedule.town_code,
                                    TownName = bandar.town_name,
                                    TypeId = (int)schedule.type_id,
                                    DeptId = (int)schedule.dept_id,
                                    CreatedAt = schedule.created_at,
                                    DistrictCode = schedule.district_code,
                                    UserId = (int)schedule.user_id,
                                    is_deleted = (bool)schedule.is_deleted,
                                    isSchedule = schedule.is_scheduled,
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
    }
}
