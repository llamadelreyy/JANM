using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;
using PBTPro.Shared.Models.RequestPayLoad;

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
                var mstLots = await _dbContext.TrnPatrols.Where(x => x.Isactive == true).AsNoTracking().ToListAsync();
                return Ok(mstLots, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
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
                string runUser = await getDefRunUser();
                List<string> teamMembers = new List<string>();
                teamMembers.Add(runUser);
                teamMembers.AddRange(InputModel.Usernames.Where(username => !string.IsNullOrWhiteSpace(username)));

                #region Validation
                var isActivePatrolling = await _dbContext.TrnPatrolDets
                                        .AnyAsync(x => teamMembers.Contains(x.Username) &&
                                           _dbContext.TrnPatrols.Any(y =>
                                               y.RecId == x.PatrolId &&
                                               y.Status.ToUpper() == "INPROGRESS"
                                           )
                                        );

                if (isActivePatrolling)
                {
                    return Error("", SystemMesg(_feature, "MEMBERS_ACTIVEPATROL", MessageTypeEnum.Error, string.Format("beberapa ahli pasukan telah tersenarai di dalam kumpulan rondaan aktif lain")));
                }
                #endregion

                #region store data
                TrnPatrol patrol = new TrnPatrol {
                    Status = "InProgress",
                    StartDtm = DateTime.Now,
                    StartLocation = InputModel.CurrentLocation,
                    CreatedBy = runUser,
                    CreatedDtm = DateTime.Now
                };

                _dbContext.TrnPatrols.Add(patrol);
                await _dbContext.SaveChangesAsync();

                List<TrnPatrolDet> patrolDets = new List<TrnPatrolDet>();
                
                foreach(var member in teamMembers)
                {
                    bool isLeader = member == runUser;
                    TrnPatrolDet patrolDet = new TrnPatrolDet
                    {
                        PatrolId = patrol.RecId,
                        Username = member,
                        Isleader = isLeader,
                        CreatedBy = runUser,
                        CreatedDtm = DateTime.Now
                    };

                    patrolDets.Add(patrolDet);
                }
                
                _dbContext.TrnPatrolDets.AddRange(patrolDets);
                await _dbContext.SaveChangesAsync();
                #endregion

                #region push data
                var memberDets = patrolDets.Join(_dbContext.Users, patrolDet => patrolDet.Username, user => user.UserName,
                    (patrolDet, user) => new {
                        Username = patrolDet.Username,
                        Isleader = patrolDet.Isleader,
                        Name = user.Name
                    }).ToList();

                foreach (var patrolDet in patrolDets.Where(x => x.Isleader != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.Username).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STRPATROL",
                            PatrolId = patrol.RecId,
                            Isleader = false,
                            Members = memberDets.Where(x => x.Username != patrolDet.Username).ToList()
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }
                #endregion

                var result = new
                {
                    Action = "STRPATROL",
                    PatrolId = patrol.RecId,
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
                string runUser = await getDefRunUser();

                #region Validation
                var patrol = await _dbContext.TrnPatrols.FirstOrDefaultAsync(x => x.RecId == InputModel.PatrolId && x.Status.ToUpper() == "INPROGRESS");

                if (patrol == null)
                {
                    return Error("", SystemMesg(_feature, "PATROL_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Rondaan tidak dijumpai")));
                }
                #endregion

                patrol.StopLocation = InputModel.CurrentLocation;
                patrol.StopDtm = DateTime.Now;
                patrol.Status = "Completed";
                patrol.ModifiedBy = runUser;
                patrol.ModifiedDtm = DateTime.Now;

                _dbContext.TrnPatrols.Update(patrol);
                await _dbContext.SaveChangesAsync();

                List<TrnPatrolDet>? patrolDets = await _dbContext.TrnPatrolDets.Where(x => x.PatrolId == InputModel.PatrolId).ToListAsync();
                foreach (var patrolDet in patrolDets.Where(x => x.Isleader != true))
                {
                    var connectionId = PushDataHub.GetConnectedUsers().Where(kvp => kvp.Value == patrolDet.Username).Select(kvp => kvp.Key).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(connectionId))
                    {
                        var data = new
                        {
                            Action = "STPPATROL",
                            PatrolId = patrol.RecId,
                            Isleader = false
                        };
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "web-api", data);
                    }
                }

                var result = new
                {
                    Action = "STPPATROL",
                    PatrolId = patrol.RecId,
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
