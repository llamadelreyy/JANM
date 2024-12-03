using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILogger<PatrolController> _logger;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "PATROL";
        public PatrolController(PBTProDbContext dbContext, ILogger<PatrolController> logger, IHubContext<PushDataHub> hubContext) : base(dbContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        #region patrol_info
        [AllowAnonymous]
        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList(string? crs = null)
        {
            try
            {               
                var data = await _dbContext.patrol_infos.Where(x => x.active_flag == true).AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        [Route("StartPatrol")]
        public async Task<IActionResult> StartPatrol([FromBody] StartPatrolModel InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);
                teamMembers.AddRange(InputModel.Usernames.Where(username => !string.IsNullOrWhiteSpace(username)));

                #region Validation
                var isActivePatrolling = await _dbContext.patrol_members
                                        .AnyAsync(x => teamMembers.Contains(x.member_username) &&
                                           _dbContext.patrol_infos.Any(y =>
                                               y.patrol_id == x.member_patrol_id &&
                                               y.patrol_status.ToUpper() == "IN-PROGRESS"
                                           )
                                        );

                if (isActivePatrolling)
                {
                    return Error("", SystemMesg(_feature, "MEMBERS_ACTIVEPATROL", MessageTypeEnum.Error, string.Format("beberapa ahli pasukan telah tersenarai di dalam kumpulan rondaan aktif lain")));
                }
                #endregion

                #region store data
                patrol_info patrol = new patrol_info {
                    patrol_status = "IN-PROGRESS",
                    patrol_start_dtm = DateTime.Now,
                    patrol_start_location = InputModel.CurrentLocation,
                    created_by = runUserID,
                    created_date = DateTime.Now
                };

                _dbContext.patrol_infos.Add(patrol);
                await _dbContext.SaveChangesAsync();

                List<patrol_member> patrolDets = new List<patrol_member>();
                
                foreach(var member in teamMembers)
                {
                    bool isLeader = member == runUser;
                    patrol_member patrolDet = new patrol_member
                    {
                        member_patrol_id = patrol.patrol_id,
                        member_username = member,
                        member_leader_flag = isLeader,
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
                    (patrolDet, user) => new {
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
        [Route("StopPatrol")]
        public async Task<IActionResult> StopPatrol([FromBody] StopPatrolModel InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _dbContext.patrol_infos.FirstOrDefaultAsync(x => x.patrol_id == InputModel.PatrolId && x.patrol_status.ToUpper() == "IN-PROGRESS");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }
                #endregion

                patrol.patrol_end_location = InputModel.CurrentLocation;
                patrol.patrol_end_dtm = DateTime.Now;
                patrol.patrol_status = "Completed";
                patrol.updated_by = runUserID;
                patrol.update_date = DateTime.Now;

                _dbContext.patrol_infos.Update(patrol);
                await _dbContext.SaveChangesAsync();

                List<patrol_member>? patrolDets = await _dbContext.patrol_members.Where(x => x.member_patrol_id == InputModel.PatrolId).ToListAsync();
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

        #endregion

        #region scheduler
        [AllowAnonymous]
        [HttpGet]
        //[Route("GetSchedulerList")]
        public async Task<IActionResult> GetSchedulerList(string? crs = null)
        {
            try
            {
                var data = await _dbContext.patrol_schedulers.Where(x => x.active_flag == true).AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostScheduler([FromBody] PatrolSchedulerModel InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);

                #region store data
                patrol_scheduler patrolscheduler = new patrol_scheduler
                {
                    scheduler_officer = InputModel.scheduler_officer,
                    scheduler_date = InputModel.scheduler_date,
                    scheduler_location = InputModel.scheduler_location,
                    created_by = runUserID,
                    created_date = DateTime.Now,
                    active_flag = true,
                };

                _dbContext.patrol_schedulers.Add(patrolscheduler);
                await _dbContext.SaveChangesAsync();
               
                #endregion

                var result = new
                {
                    scheduler_officer = patrolscheduler.scheduler_officer,
                    scheduler_date = patrolscheduler.scheduler_date,
                    scheduler_id = patrolscheduler.scheduler_id                    
                };

                return Ok(result, SystemMesg(_feature, "CREATE_PATROL_SCHEDULER", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] patrol_scheduler InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.patrol_schedulers.FirstOrDefaultAsync(x => x.scheduler_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }                

                if (string.IsNullOrWhiteSpace(InputModel.scheduler_officer))
                {
                    return Error("", SystemMesg(_feature, "SCHEDULER_OFFICER_NAME", MessageTypeEnum.Error, string.Format("Ruangan Nama Pegawai diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.scheduler_location))
                {
                    return Error("", SystemMesg(_feature, "SCHEDULER_LOCATION", MessageTypeEnum.Error, string.Format("Ruangan Lokasi Rondaan diperlukan")));
                }
                if (InputModel.scheduler_date < DateTime.Today)
                {
                    return Error("", SystemMesg(_feature, "SCHEDULER_DATE", MessageTypeEnum.Error, string.Format("Ruangan Tarikh Rondaan tidak boleh kurang daripada tarikh hari ini.")));
                }

                #endregion

                formField.scheduler_officer = InputModel.scheduler_officer;
                formField.scheduler_location = InputModel.scheduler_location;
                formField.scheduler_date = InputModel.scheduler_date?.ToLocalTime();


                formField.updated_by = runUserID;
                formField.update_date = DateTime.Now;

                _dbContext.patrol_schedulers.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Remove(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.patrol_schedulers.FirstOrDefaultAsync(x => x.scheduler_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.patrol_schedulers.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        #region lookup data
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetOfficerName(string? crs = null)
        {
            ReturnViewModel response = new ReturnViewModel();

            try
            {
                var data = await _dbContext.patrol_schedulers
                        .Where(x => x.active_flag == true)
                        //.Join(
                        //    _dbContext.patrol_infos,
                        //    scheduler => scheduler.scheduler_id, // assuming patrol_id is the common field
                        //    info => info.scheduler_id,           // assuming patrol_id is the common field
                        //    (scheduler, info) => new
                        //    {
                        //        Scheduler = scheduler,
                        //        Info = info
                        //    }
                        //)
                        .AsNoTracking()
                        .ToListAsync();

                var result = data.Select(x => new
                {      
                    x.scheduler_id,
                    x.scheduler_officer,  
                                    
                }).ToList();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        [AllowAnonymous]        
        [HttpGet("{offName}")]
        public async Task<IActionResult> GetPatrolLocation(string? crs = null, string offName="")
        {
            ReturnViewModel response = new ReturnViewModel();

            try
            {
                var data = await _dbContext.patrol_schedulers
                        .Where(x => x.active_flag == true && x.scheduler_officer == offName)                        
                        .AsNoTracking()
                        .ToListAsync();

                var result = data.Select(x => new
                {
                    x.scheduler_id,
                    x.scheduler_location,

                }).ToList();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion
    }
}
