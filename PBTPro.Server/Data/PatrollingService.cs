/*
Project: PBT Pro
Description: Patrolling Schedule service to handle patrolling form field
Author: Nurulfarhana
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBT.Data;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBT.Data
{
    [AllowAnonymous]
    public partial class PatrollingService : IDisposable
    {

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                }

                // Dispose unmanaged resources here

                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public IConfiguration _configuration { get; }
        private List<PatrollingInfo> _Patrolling { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<PatrollingService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public PatrollingService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<PatrollingService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            //_Patrolling = await GetAllPatrolling();
        }
        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<PatrollingInfo>> GetAllPatrolling()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Patrolling/ListPatrolling");
                string jsonString = await _cf.List(request);
                List<PatrollingInfo> patrollingList = JsonConvert.DeserializeObject<List<PatrollingInfo>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai jadual rondaan.", 1, LoggerName, "");

                return patrollingList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<PatrollingInfo>> RefreshListPatrolling()
        {
            GetDefaultPermission();
            //var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Patrolling/ListPatrolling");
                string jsonString = await _cf.List(request);

                List<PatrollingInfo> patrollingList = JsonConvert.DeserializeObject<List<PatrollingInfo>>(jsonString);
                 await _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - RefreshListPatrolling", "Papar semula senarai jadual rondaan.", 1, LoggerName, "");

                return patrollingList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "PatrollingService - RefreshListPatrolling", ex.Message, 1, LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<PatrollingInfo> GetIdPatrolling(int id)
        {
            GetDefaultPermission();
            //var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Patrolling/RetrievePatrolling/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);
                PatrollingInfo patrolling = JsonConvert.DeserializeObject<PatrollingInfo>(jsonString.ToString());
                await _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - GetIdPatrolling", "Papar maklumat terperinci jadual rondaan.", 1, LoggerName, "");
                
                return patrolling;
            }
            catch (Exception ex) 
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "PatrollingService - GetIdPatrolling", ex.Message, 1, LoggerName, "");
                return null;
            }
        }        
                    
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<PatrollingInfo>> PostPatrolling([FromBody] string patrollings= "")
        {
            GetDefaultPermission();
            //var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Patrolling/InsertPatrolling");
                string jsonString = await _cf.AddNew(request, patrollings, platformApiUrl + "/api/Patrolling/InsertPatrolling");
                PatrollingInfo patrolling = JsonConvert.DeserializeObject<PatrollingInfo>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - PostPatrolling", "Tambah data baru untuk jadual rondaan.", 1, LoggerName, "");
                
                return patrolling;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "PatrollingService - PostPatrolling", ex.Message, 1, LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeletePatrolling(int id)
        {
            GetDefaultPermission();
            //var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Patrolling/DeletePatrolling/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - DeletePatrolling", "Berjaya padam data untuk jadual rondaan.", 1, LoggerName, "");

                return id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "PatrollingService - DeletePatrolling", ex.Message, 1, LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult<PatrollingInfo>> PutPatrolling(int id, PatrollingInfo patrolling)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Patrolling/UpdatePatrolling/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Patrolling/UpdatePatrolling/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(patrolling), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - PutPatrolling", "Berjaya kemaskini data untuk jadual rondaan.", 1, LoggerName, "");
                
                return patrolling;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "PatrollingService - PutPatrolling", ex.Message, 1, LoggerName, "");
                return null;
            }
        }
        public Task<List<PatrollingInfo>> GetPatrollingAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, "PatrollingService - GetPatrollingAsync", "Berjaya muat semula senarai untuk jadual rondaan.", 1, LoggerName, "");

            return Task.FromResult(_Patrolling);
        }
    }
}
