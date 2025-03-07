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
31/01/2025 (Author: Fakhrul) - add sort by patrolid in GetDateTimeByUserId function to return the latest patrol with status SELESAI for current user
31/01/2025 (Author: Fakhrul) - add new function called GetPatrolStatusByUserId to display current user latest patrol information
20/2/2024 - change dbcontext to mst_patrol_schedule & trn_patrol_officers. 
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PatrolsController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "PATROLS";
        private readonly ILogger<PatrolsController> _logger;

        public PatrolsController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<PatrolsController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
            _tenantDBContext = tntdbContext;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetList(string? crs = null)
        {
            try
            {
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
                    creator_id = x.creator_id,
                    created_at = x.created_at,
                    modifier_id = x.modifier_id,
                    modified_at = x.modified_at,
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
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var patrol = await _tenantDBContext.mst_patrol_schedules.Where(x => x.schedule_id == Id).Select(x => new
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
                    creator_id = x.creator_id,
                    created_at = x.created_at,
                    modifier_id = x.modifier_id,
                    modified_at = x.modified_at,
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

                }).FirstOrDefaultAsync();

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                var members = await (from pm in _tenantDBContext.trn_patrol_officers
                                     join u in _dbContext.Users on pm.idno equals u.UserName
                                     where pm.schedule_id == patrol.schedule_id
                                     select new
                                     {
                                         pm.idno,
                                         pm.schedule_id,
                                         pm.officer_id,
                                         pm.cnt_notice,
                                         pm.cnt_cmpd,
                                         pm.cnt_notes,
                                         pm.cnt_seizure,
                                         pm.creator_id,
                                         pm.created_at,
                                         pm.modifier_id,
                                         pm.modified_at,
                                         pm.start_time,
                                         pm.end_time,
                                         member_fullname = u.UserName,
                                         pm.user_id
                                     })
                                    .AsNoTracking()
                                    .ToListAsync();

                var result = new
                {
                    info = patrol,
                    members = members
                };

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
       

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] PatrolInputMemberModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.username))
                {
                    return Error("", SystemMesg(_feature, "MEMBER_ISNULL", MessageTypeEnum.Error, string.Format("pegawai tidah sah")));
                }

                var patrol = await _dbContext.patrol_infos.FirstOrDefaultAsync(x => x.patrol_id == InputModel.patrol_id && x.patrol_status.ToUpper() == "RONDAAN");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                var isActivePatrolling = await _tenantDBContext.trn_patrol_officers
                                        .AnyAsync(x => x.idno == InputModel.username && x.end_time == null &&
                                           _tenantDBContext.mst_patrol_schedules.Any(y =>
                                               y.schedule_id == x.schedule_id &&
                                               y.status_id == 2
                                           )
                                        );

                if (isActivePatrolling)
                {
                    return Error("", SystemMesg(_feature, "MEMBER_ACTIVEPATROL", MessageTypeEnum.Error, string.Format("pegawai telah tersenarai di dalam kumpulan rondaan aktif lain")));
                }
                #endregion

                trn_patrol_officer patrolDet = new trn_patrol_officer
                {
                    schedule_id = patrol.patrol_id,
                    idno = InputModel.username,
                    is_leader = false,
                    start_time = DateTime.Now,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _tenantDBContext.trn_patrol_officers.Add(patrolDet);
                await _tenantDBContext.SaveChangesAsync();

                var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.idno).Select(kvp => kvp.Key).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    var data = new
                    {
                        Action = "STRPATROL",
                        PatrolId = patrol.patrol_id,
                        Isleader = false
                    };
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                }

                var result = new
                {
                    Action = "ADDMEMBER",
                    PatrolId = patrol.patrol_id,
                    Isleader = true
                };
                return Ok(result, SystemMesg(_feature, "PATROL_ADD_MEMBER", MessageTypeEnum.Success, string.Format("Berjaya menambah pegawai")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember([FromBody] PatrolInputMemberModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _tenantDBContext.mst_patrol_schedules.FirstOrDefaultAsync(x => x.schedule_id == InputModel.patrol_id && x.status_id == 2);

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                trn_patrol_officer patrolDet = await _tenantDBContext.trn_patrol_officers.FirstOrDefaultAsync(x => x.schedule_id == InputModel.patrol_id && x.idno == InputModel.username && x.end_time == null);

                if (patrolDet == null)
                {

                    return Error("", SystemMesg(_feature, "PATROL_MEMBER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pegawai tidak dijumpai")));
                }
                #endregion

                patrolDet.end_time = DateTime.Now;
                patrolDet.modifier_id = runUserID;
                patrolDet.modified_at = DateTime.Now;
                _tenantDBContext.trn_patrol_officers.Update(patrolDet);

                await _tenantDBContext.SaveChangesAsync();

                var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.idno).Select(kvp => kvp.Key).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    var data = new
                    {
                        Action = "STPPATROL",
                        PatrolId = patrol.schedule_id,
                        Isleader = false
                    };
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                }

                var result = new
                {
                    Action = "REMMEMBER",
                    PatrolId = patrol.schedule_id,
                    Isleader = true
                };
                return Ok(result, SystemMesg(_feature, "PATROL_REM_MEMBER", MessageTypeEnum.Success, string.Format("Berjaya membuang pegawai")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetDateTimeByUserId(string username)
        {
            try
            {
                var patrol_member = await (from p in _tenantDBContext.mst_patrol_schedules
                                           join pm in _tenantDBContext.trn_patrol_officers on p.schedule_id equals pm.schedule_id
                                           where p.status_id == 3 && pm.idno == username
                                           orderby p.schedule_id descending
                                           select new
                                           {
                                               p.start_time,
                                               pm.end_time,
                                               PatrolDuration = p.start_time != null && pm.end_time != null
                                                ? (pm.end_time - p.start_time)
                                                : (TimeSpan?)null
                                           }).FirstOrDefaultAsync();

                return Ok(patrol_member, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListbyCurrentUser()
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                var patrol = await _tenantDBContext.mst_patrol_schedules.Where(x => x.start_time == DateTime.Today && x.creator_id == runUserID).Select(x => new
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
                    creator_id = x.creator_id,
                    created_at = x.created_at,
                    modifier_id = x.modifier_id,
                    modified_at = x.modified_at,
                    is_deleted = false,
                    start_location = x.start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.start_location))
                                    : null,

                    end_location = x.end_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.end_location))
                                    : null,
                    district_code = x.district_code,
                    town_code = x.town_code,
                }).AsNoTracking().ToListAsync();

                return Ok(patrol, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, "Senarai rekod berjaya dijana"));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, "Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian."));
            }
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetPatrolStatusByUserId(string username)
        {
            try
            {
                var patrol_member = await (from p in _tenantDBContext.mst_patrol_schedules
                                           join pm in _tenantDBContext.trn_patrol_officers on p.schedule_id equals pm.schedule_id
                                           where pm.idno == username
                                           orderby p.schedule_id descending
                                           select new
                                           {
                                               p.schedule_id,
                                               p.status_id,
                                               pm.idno,
                                               p.start_time,
                                               pm.end_time,
                                               PatrolDuration = p.start_time != null && pm.end_time != null
                                                ? (pm.end_time - p.start_time)
                                                : (TimeSpan?)null
                                           }).FirstOrDefaultAsync();

                if (patrol_member == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_MEMBER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pegawai tidak dijumpai")));
                }

                var patrol_member_leader = await (from p in _tenantDBContext.mst_patrol_schedules
                                                  join pm in _tenantDBContext.trn_patrol_officers on p.schedule_id equals pm.schedule_id
                                                  where p.schedule_id == patrol_member.schedule_id
                                                  orderby p.schedule_id descending
                                                  select new
                                                  {
                                                      pm.idno,
                                                  }).FirstOrDefaultAsync();

                var totalMembers = await (from p in _tenantDBContext.mst_patrol_schedules
                                          join pm in _tenantDBContext.trn_patrol_officers on p.schedule_id equals pm.schedule_id
                                          where pm.schedule_id == patrol_member.schedule_id
                                          select pm).CountAsync();

                var resultData = new
                {
                    patrol_id = patrol_member.schedule_id,
                    patrol_leader = patrol_member_leader?.idno, 
                    total_member = totalMembers,
                    patrol_status = patrol_member.status_id,
                    current_user = patrol_member.idno,
                    patrol_start_dtm = patrol_member.start_time,
                    member_end_dtm = patrol_member.end_time,
                    patrolDuration = patrol_member.PatrolDuration,
                };

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartPatrol([FromBody] StartPatrolModel InputModel)
        {
            try
            {
                bool isNew = false;
                mst_patrol_schedule patrol = new mst_patrol_schedule();
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);
                teamMembers.AddRange(InputModel.usernames.Where(username => !string.IsNullOrWhiteSpace(username) && username != runUser));

                #region Validation
                if (InputModel.patrol_id.HasValue)
                {
                    patrol = await _tenantDBContext.mst_patrol_schedules.Where(x => x.schedule_id == InputModel.patrol_id).FirstOrDefaultAsync();
                    if (patrol == null)
                    {
                        isNew = true;
                    }
                }
                else
                {
                    patrol = new mst_patrol_schedule
                    {
                        creator_id = runUserID,
                        created_at = DateTime.Now,
                        status_id = 3,//"Belum Mula",
                        is_scheduled = false
                    };

                    isNew = true;
                }

                if (patrol.status_id!= 3)
                {
                    return Error("", SystemMesg(_feature, "PATROL_ISNOT_NEW", MessageTypeEnum.Error, string.Format("rondaan sedang aktif dilaksanakan atau telah selesai")));
                }

                var isActivePatrolling = await _tenantDBContext.trn_patrol_officers
                                        .AnyAsync(x => teamMembers.Contains(x.idno) && x.end_time == null &&
                                           _tenantDBContext.mst_patrol_schedules.Any(y =>
                                               y.schedule_id == x.schedule_id &&
                                               y.status_id == 2//"RONDAAN"
                                           )
                                        );

                if (isActivePatrolling)
                {
                    return Error("", SystemMesg(_feature, "MEMBERS_ACTIVEPATROL", MessageTypeEnum.Error, string.Format("beberapa ahli pasukan telah tersenarai di dalam kumpulan rondaan aktif lain")));
                }
                #endregion

                #region store data
                var CurrentLocationArr = InputModel.current_location;
                var CurrentLocation = new Point(CurrentLocationArr.Longitude, CurrentLocationArr.Latitude)
                {
                    SRID = 4326
                };

                patrol.status_id = 2;// "Rondaan";
                patrol.start_time = DateTime.Now;
                patrol.start_location = CurrentLocation;
                patrol.modifier_id = runUserID;
                patrol.modified_at = DateTime.Now;

                if (isNew == true)
                {
                    _tenantDBContext.mst_patrol_schedules.Add(patrol);
                }
                else
                {
                    _tenantDBContext.mst_patrol_schedules.Update(patrol);
                }
                await _tenantDBContext.SaveChangesAsync();

                List<trn_patrol_officer> patrolDets = new List<trn_patrol_officer>();

                foreach (var member in teamMembers)
                {
                    bool isLeader = member == runUser;
                    trn_patrol_officer patrolDet = new trn_patrol_officer
                    {
                        schedule_id = patrol.schedule_id,
                        idno = member,
                        is_leader = isLeader,
                        start_time = patrol.start_time,
                        creator_id = runUserID,
                        created_at = DateTime.Now
                    };

                    patrolDets.Add(patrolDet);
                }

                _tenantDBContext.trn_patrol_officers.AddRange(patrolDets);
                await _tenantDBContext.SaveChangesAsync();
                #endregion

                #region push data
                var memberDets = patrolDets.Join(_dbContext.Users, patrolDet => patrolDet.idno, user => user.UserName,
                    (patrolDet, user) => new
                    {
                        Username = patrolDet.idno,
                        Isleader = patrolDet.is_leader,
                        Name = user.UserName
                    }).ToList();

                foreach (var patrolDet in patrolDets.Where(x => x.is_leader != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.idno).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STRPATROL",
                            PatrolId = patrol.schedule_id,
                            Isleader = false,
                            Members = memberDets.Where(x => x.Username != patrolDet.idno).ToList()
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }
                #endregion

                var result = new
                {
                    Action = "STRPATROL",
                    PatrolId = patrol.schedule_id,
                    Isleader = true,
                    Members = memberDets.Where(x => x.Username != runUser).ToList()
                };
                return Ok(result, SystemMesg(_feature, "START_PATROL", MessageTypeEnum.Success, string.Format("Berjaya memulakan rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> StopPatrol([FromBody] StopPatrolModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _tenantDBContext.mst_patrol_schedules.FirstOrDefaultAsync(x => x.schedule_id == InputModel.patrol_id && x.status_id == 2);//"RONDAAN");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }
                #endregion

                var CurrentLocationArr = InputModel.current_location;
                var CurrentLocation = new Point(CurrentLocationArr.Longitude, CurrentLocationArr.Latitude)
                {
                    SRID = 4326
                };

                patrol.end_location = CurrentLocation;
                patrol.end_time = DateTime.Now;
                patrol.status_id = 1;//"Selesai";
                patrol.modifier_id = runUserID;
                patrol.modified_at = DateTime.Now;

                _tenantDBContext.mst_patrol_schedules.Update(patrol);
                await _tenantDBContext.SaveChangesAsync();

                List<trn_patrol_officer>? patrolDets = await _tenantDBContext.trn_patrol_officers.Where(x => x.schedule_id == InputModel.patrol_id).ToListAsync();
                foreach (var patrolDet in patrolDets)
                {
                    patrolDet.end_time = patrol.end_time;
                    patrolDet.modifier_id = runUserID;
                    patrolDet.modified_at = DateTime.Now;
                    _tenantDBContext.trn_patrol_officers.Update(patrolDet);
                }
                await _tenantDBContext.SaveChangesAsync();

                foreach (var patrolDet in patrolDets.Where(x => x.is_leader != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.idno).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STPPATROL",
                            PatrolId = patrol.schedule_id,
                            Isleader = false
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }

                var result = new
                {
                    Action = "STPPATROL",
                    PatrolId = patrol.schedule_id,
                    Isleader = true
                };
                return Ok(result, SystemMesg(_feature, "STOP_PATROL", MessageTypeEnum.Success, string.Format("Berjaya menghentikan rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

    }
}
