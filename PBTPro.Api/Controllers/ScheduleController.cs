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


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ScheduleController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;

        private string LoggerName = "administrator";
        private readonly string _feature = "SCHEDULE";

        public ScheduleController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<ScheduleController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
        }

        #region patrol_info
        [AllowAnonymous]
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
                    is_scheduled = false,
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
                                    : null
                }).AsNoTracking().ToListAsync();

                var result = (
                        from schedule in data
                        join user in _dbContext.Users on schedule.idno equals user.IdNo
                        join department in _tenantDBContext.ref_departments on user.dept_id equals department.dept_id
                        join seksyen in _tenantDBContext.ref_divisions on user.div_id equals seksyen.div_id
                        join unit in  _tenantDBContext.ref_units on user.unit_id equals unit.unit_id
                        
                        select new PatrolViewModel
                        {        
                            scheduleId = schedule.schedule_id,
                            OfficerName = user.full_name,
                            CreatedAt = schedule.created_at,
                            DeptName = department.dept_name,
                            SectionName = seksyen.div_name,
                            UnitName = unit.unit_name,
                            StartTime = schedule.start_time,
                            EndTime = schedule.end_time,
                            DistrictName = schedule.loc_name,
                        }
                    ).ToList();


                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
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
               
                #region store data
                mst_patrol_schedule mst_Patrol = new mst_patrol_schedule
                {
                    idno = InputModel.ICNo,
                    dept_id = InputModel.DeptId,
                    start_time = InputModel.StartTime,
                    end_time = InputModel.EndTime,
                    creator_id = runUserID,
                    created_at = InputModel.CreatedAt,
                    is_deleted = false,
                    //need to add new field in the tbl district and town, rename loc_name
                    loc_name = InputModel.DistrictName,
                    
                };

                _tenantDBContext.mst_patrol_schedules.Add(mst_Patrol);
                await _tenantDBContext.SaveChangesAsync();

                #endregion               
                return Ok(mst_Patrol, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya tambah jadual rondaan")));
            }
            catch (Exception ex)
            {
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

                #endregion
                formField.idno = InputModel.ICNo;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.mst_patrol_schedules.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
    }
}
