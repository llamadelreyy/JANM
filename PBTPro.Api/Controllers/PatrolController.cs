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
    [Route("api/[controller]")]
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


    }
}
