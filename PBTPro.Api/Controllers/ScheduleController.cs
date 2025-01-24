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
        protected PBTProTenantDbContext _tenantDBContext;

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
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
    }
}
