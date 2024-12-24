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
    public class PatrolController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;

        private string LoggerName = "administrator";
        private readonly string _feature = "PATROL";

        public PatrolController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<PatrolController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
        }

        #region patrol_info
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetList(string? crs = null)
        {
            try
            {
                var data = await _dbContext.patrol_infos.Where(x => x.active_flag == true).Select(x => new 
                            {
                                patrol_id = x.patrol_id,
                                patrol_cnt_notice = x.patrol_cnt_notice,
                                patrol_cnt_compound = x.patrol_cnt_compound,
                                patrol_cnt_notes = x.patrol_cnt_notes,
                                patrol_cnt_seizure = x.patrol_cnt_seizure,
                                patrol_status = x.patrol_status,
                                patrol_start_dtm = x.patrol_start_dtm,
                                patrol_end_dtm = x.patrol_end_dtm,
                                active_flag = x.active_flag,
                                created_by = x.created_by,
                                created_date = x.created_date,
                                updated_by = x.updated_by,
                                patrol_officer_name = x.patrol_officer_name,
                                patrol_location = x.patrol_location,
                                updated_date = x.updated_date,
                                patrol_dept_name = x.patrol_dept_name,
                                patrol_scheduled = x.patrol_scheduled,
                                patrol_start_location = x.patrol_start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.patrol_start_location))
                                    : null,

                                patrol_end_location = x.patrol_end_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.patrol_end_location))
                                    : null
                            }).AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        
        [HttpGet]
        //[Route("GetDetail")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var patrol = await _dbContext.patrol_infos.Where(x => x.patrol_id == Id).Select(x => new 
                            {
                                patrol_id = x.patrol_id,
                                patrol_cnt_notice = x.patrol_cnt_notice,
                                patrol_cnt_compound = x.patrol_cnt_compound,
                                patrol_cnt_notes = x.patrol_cnt_notes,
                                patrol_cnt_seizure = x.patrol_cnt_seizure,
                                patrol_status = x.patrol_status,
                                patrol_start_dtm = x.patrol_start_dtm,
                                patrol_end_dtm = x.patrol_end_dtm,
                                active_flag = x.active_flag,
                                created_by = x.created_by,
                                created_date = x.created_date,
                                updated_by = x.updated_by,
                                patrol_officer_name = x.patrol_officer_name,
                                patrol_location = x.patrol_location,
                                updated_date = x.updated_date,
                                patrol_dept_name = x.patrol_dept_name,
                                patrol_scheduled = x.patrol_scheduled,
                                patrol_start_location = x.patrol_start_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.patrol_start_location))
                                    : null,

                                patrol_end_location = x.patrol_end_location != null
                                    ? PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.patrol_end_location))
                                    : null
                            }).FirstOrDefaultAsync();
                
                if(patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                var members = await (from pm in _dbContext.patrol_members
                                    join u in _dbContext.Users on pm.member_username equals u.UserName
                                    where pm.member_patrol_id == patrol.patrol_id
                                    select new
                                    {
                                        pm.member_id,
                                        pm.member_patrol_id,
                                        pm.member_username,
                                        pm.member_cnt_notice,
                                        pm.member_cnt_compound,
                                        pm.member_cnt_notes,
                                        pm.member_cnt_seizure,
                                        pm.member_leader_flag,
                                        pm.active_flag,
                                        pm.created_by,
                                        pm.created_date,
                                        pm.updated_by,
                                        pm.update_date,
                                        pm.member_start_dtm,
                                        pm.member_end_dtm,
                                        member_fullname = u.Name
                                    })
                                    .AsNoTracking()
                                    .ToListAsync();

                var result = new {
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
        //[Route("StartPatrol")]
        public async Task<IActionResult> StartPatrol([FromBody] StartPatrolModel InputModel)
        {
            try
            {
                bool isNew = false;
                patrol_info patrol = new patrol_info();
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);
                teamMembers.AddRange(InputModel.usernames.Where(username => !string.IsNullOrWhiteSpace(username) && username != runUser));
                
                #region Validation
                if(InputModel.patrol_id.HasValue)
                {
                    patrol = await _dbContext.patrol_infos.Where(x=>x.patrol_id == InputModel.patrol_id).FirstOrDefaultAsync();
                    if(patrol == null)
                    {
                        isNew = true;
                    }
                }else{
                    patrol = new patrol_info
                    {
                        created_by = runUserID,
                        created_date = DateTime.Now,
                        patrol_status = "Belum Mula",
                        patrol_scheduled = false
                    };

                    isNew = true;
                }
                
                if(patrol.patrol_status.ToUpper() != "BELUM MULA")
                {
                    return Error("", SystemMesg(_feature, "PATROL_ISNOT_NEW", MessageTypeEnum.Error, string.Format("rondaan sedang aktif dilaksanakan atau telah selesai")));
                }

                var isActivePatrolling = await _dbContext.patrol_members
                                        .AnyAsync(x => teamMembers.Contains(x.member_username) && x.member_end_dtm == null && 
                                           _dbContext.patrol_infos.Any(y =>
                                               y.patrol_id == x.member_patrol_id &&
                                               y.patrol_status.ToUpper() == "RONDAAN"
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
                
                patrol.patrol_status = "Rondaan";
                patrol.patrol_start_dtm = DateTime.Now;
                patrol.patrol_start_location = CurrentLocation;
                patrol.updated_by = runUserID;
                patrol.updated_date = DateTime.Now;
                
                if(isNew == true)
                {
                    _dbContext.patrol_infos.Add(patrol);
                }else{
                    _dbContext.patrol_infos.Update(patrol);
                }
                await _dbContext.SaveChangesAsync();

                List<patrol_member> patrolDets = new List<patrol_member>();

                foreach (var member in teamMembers)
                {
                    bool isLeader = member == runUser;
                    patrol_member patrolDet = new patrol_member
                    {
                        member_patrol_id = patrol.patrol_id,
                        member_username = member,
                        member_leader_flag = isLeader,
                        member_start_dtm = patrol.patrol_start_dtm,
                        created_by = runUserID,
                        created_date = DateTime.Now
                    };

                    patrolDets.Add(patrolDet);
                }

                _dbContext.patrol_members.AddRange(patrolDets);
                await _dbContext.SaveChangesAsync();
                #endregion

                #region push data
                var memberDets = patrolDets.Join(_dbContext.Users, patrolDet => patrolDet.member_username, user => user.UserName,
                    (patrolDet, user) => new
                    {
                        Username = patrolDet.member_username,
                        Isleader = patrolDet.member_leader_flag,
                        Name = user.Name
                    }).ToList();

                foreach (var patrolDet in patrolDets.Where(x => x.member_leader_flag != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.member_username).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STRPATROL",
                            PatrolId = patrol.patrol_id,
                            Isleader = false,
                            Members = memberDets.Where(x => x.Username != patrolDet.member_username).ToList()
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }
                #endregion

                var result = new
                {
                    Action = "STRPATROL",
                    PatrolId = patrol.patrol_id,
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
        //[Route("StopPatrol")]
        public async Task<IActionResult> StopPatrol([FromBody] StopPatrolModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _dbContext.patrol_infos.FirstOrDefaultAsync(x => x.patrol_id == InputModel.patrol_id && x.patrol_status.ToUpper() == "RONDAAN");

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

                patrol.patrol_end_location = CurrentLocation;
                patrol.patrol_end_dtm = DateTime.Now;
                patrol.patrol_status = "Selesai";
                patrol.updated_by = runUserID;
                patrol.updated_date = DateTime.Now;

                _dbContext.patrol_infos.Update(patrol);
                await _dbContext.SaveChangesAsync();

                List<patrol_member>? patrolDets = await _dbContext.patrol_members.Where(x => x.member_patrol_id == InputModel.patrol_id).ToListAsync();
                foreach (var patrolDet in patrolDets)
                {
                    patrolDet.member_end_dtm = patrol.patrol_end_dtm;
                    patrolDet.updated_by = runUserID;
                    patrolDet.update_date = DateTime.Now;
                    _dbContext.patrol_members.Update(patrolDet);
                }                    
                await _dbContext.SaveChangesAsync();

                foreach (var patrolDet in patrolDets.Where(x => x.member_leader_flag != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.member_username).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STPPATROL",
                            PatrolId = patrol.patrol_id,
                            Isleader = false
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }

                var result = new
                {
                    Action = "STPPATROL",
                    PatrolId = patrol.patrol_id,
                    Isleader = true
                };
                return Ok(result, SystemMesg(_feature, "STOP_PATROL", MessageTypeEnum.Success, string.Format("Berjaya menghentikan rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        
        [HttpPost]
        //[Route("AddMember")]
        public async Task<IActionResult> AddMember([FromBody] PatrolInputMemberModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if(string.IsNullOrWhiteSpace(InputModel.username)){                    
                    return Error("", SystemMesg(_feature, "MEMBER_ISNULL", MessageTypeEnum.Error, string.Format("pegawai tidah sah")));
                }

                var patrol = await _dbContext.patrol_infos.FirstOrDefaultAsync(x => x.patrol_id == InputModel.patrol_id && x.patrol_status.ToUpper() == "RONDAAN");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                var isActivePatrolling = await _dbContext.patrol_members
                                        .AnyAsync(x => x.member_username == InputModel.username && x.member_end_dtm == null && 
                                           _dbContext.patrol_infos.Any(y =>
                                               y.patrol_id == x.member_patrol_id &&
                                               y.patrol_status.ToUpper() == "RONDAAN"
                                           )
                                        );

                if (isActivePatrolling)
                {
                    return Error("", SystemMesg(_feature, "MEMBER_ACTIVEPATROL", MessageTypeEnum.Error, string.Format("pegawai telah tersenarai di dalam kumpulan rondaan aktif lain")));
                }
                #endregion

                patrol_member patrolDet = new patrol_member
                {
                    member_patrol_id = patrol.patrol_id,
                    member_username = InputModel.username,
                    member_leader_flag = false,
                    member_start_dtm = DateTime.Now,
                    created_by = runUserID,
                    created_date = DateTime.Now
                };

                _dbContext.patrol_members.Add(patrolDet);
                await _dbContext.SaveChangesAsync();
                
                var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.member_username).Select(kvp => kvp.Key).FirstOrDefault();
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
        //[Route("RemoveMember")]
        public async Task<IActionResult> RemoveMember([FromBody] PatrolInputMemberModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _dbContext.patrol_infos.FirstOrDefaultAsync(x => x.patrol_id == InputModel.patrol_id && x.patrol_status.ToUpper() == "RONDAAN");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }

                patrol_member patrolDet = await _dbContext.patrol_members.FirstOrDefaultAsync(x => x.member_patrol_id == InputModel.patrol_id && x.member_username == InputModel.username && x.member_end_dtm == null);

                if(patrolDet == null){

                    return Error("", SystemMesg(_feature, "PATROL_MEMBER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pegawai tidak dijumpai")));
                }
                #endregion

                patrolDet.member_end_dtm = DateTime.Now;
                patrolDet.updated_by = runUserID;
                patrolDet.update_date = DateTime.Now;
                _dbContext.patrol_members.Update(patrolDet);
                    
                await _dbContext.SaveChangesAsync();
                
                var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.member_username).Select(kvp => kvp.Key).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    var data = new
                    {
                        Action = "STPPATROL",
                        PatrolId = patrol.patrol_id,
                        Isleader = false
                    };
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                }

                var result = new
                {
                    Action = "REMMEMBER",
                    PatrolId = patrol.patrol_id,
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<patrol_info>>> ListAll()
        {
            try
            {
                var data = await _dbContext.patrol_infos.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        
        public async Task<IActionResult> Add([FromBody] patrol_info InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                var isPatrolDup = await _dbContext.patrol_infos
                                        .AnyAsync(y => y.patrol_officer_name == InputModel.patrol_officer_name &&
                                        (y.patrol_start_dtm < DateTime.Now || y.patrol_start_dtm == InputModel.patrol_start_dtm) &&
                                        (y.patrol_end_dtm < DateTime.Now || y.patrol_end_dtm == InputModel.patrol_end_dtm) &&
                                        y.patrol_location == InputModel.patrol_location);

                if (isPatrolDup)
                {
                    return Error("", SystemMesg(_feature, "MEMBERS_ASSIGNED", MessageTypeEnum.Error, string.Format("pegawai sedang dalam rondaan")));
                }
                #endregion

                #region store data
                patrol_info patrolinfo = new patrol_info
                {
                    patrol_id = InputModel.patrol_id,
                    patrol_dept_name = InputModel.patrol_dept_name,
                    patrol_officer_name = InputModel.patrol_officer_name,
                    patrol_location = InputModel.patrol_location,
                    patrol_start_dtm = InputModel.patrol_start_dtm,
                    patrol_end_dtm = InputModel.patrol_end_dtm,
                    patrol_status = InputModel.patrol_status,
                    created_by = runUserID,
                    created_date = DateTime.Now,
                    active_flag = true,
                };

                _dbContext.patrol_infos.Add(patrolinfo);
                await _dbContext.SaveChangesAsync();

                #endregion               
                return Ok(patrolinfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya tambah jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] patrol_info InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.patrol_infos
                                .FirstOrDefaultAsync(x => x.patrol_id == InputModel.patrol_id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.patrol_officer_name))
                {
                    return Error("", SystemMesg(_feature, "PATROL_OFFICER_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Pegawai diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.patrol_location))
                {
                    return Error("", SystemMesg(_feature, "PATROL_LOCATION", MessageTypeEnum.Error, string.Format("Ruangan Lokasi Rondaan diperlukan")));
                }
                #endregion
                formField.patrol_dept_name = InputModel.patrol_dept_name;
                formField.patrol_officer_name = InputModel.patrol_officer_name;
                formField.patrol_location = InputModel.patrol_location;
                formField.patrol_start_dtm = InputModel.patrol_start_dtm;
                formField.patrol_end_dtm = InputModel.patrol_end_dtm;
                formField.patrol_status = InputModel.patrol_status;
                formField.updated_by = runUserID;
                formField.updated_date = DateTime.Now;

                _dbContext.patrol_infos.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.patrol_infos.Where(x => x.patrol_status == "Belum Mula").FirstOrDefaultAsync(x => x.patrol_id == Id);

                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                #endregion

                _dbContext.patrol_infos.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListByTabType(string tabType)
        {
            try
            {
                var parFormfields = await _dbContext.patrol_infos.Where(x => x.active_flag == true && x.patrol_status == tabType)
                                    .AsNoTracking().ToListAsync();

                if (parFormfields.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(parFormfields, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
    }
}
