using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBTPro.Data
{
    [AllowAnonymous]
    public class UnitService : IDisposable
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
        private List<unit_info> _Unit { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<UnitService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public UnitService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UnitService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        }
        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<unit_info>> GetAllUnit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/ListUnit");
                string jsonString = await _cf.List(request);
                List<unit_info> unitList = JsonConvert.DeserializeObject<List<unit_info>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai unit.", Convert.ToInt32(uID), LoggerName, "");

                return unitList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<unit_info>> RefreshListUnit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/ListUnit");
                string jsonString = await _cf.List(request);
                List<unit_info> unitList = JsonConvert.DeserializeObject<List<unit_info>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai unit.", Convert.ToInt32(uID), LoggerName, "");

                return unitList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<unit_info>> AddUnit([FromBody] string units = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/InsertUnit");
                string jsonString = await _cf.AddNew(request, units, platformApiUrl + "/api/Unit/InsertUnit");
                unit_info unitList = JsonConvert.DeserializeObject<unit_info>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah data baru untuk unit.", Convert.ToInt32(uID), LoggerName, "");

                return unitList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteUnit(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/DeleteUnit/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk unit.", Convert.ToInt32(uID), LoggerName, "");

                return id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult<unit_info>> UpdateUnit(int id, unit_info unit)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Unit/UpdateUnit/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Unit/UpdateUnit/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(unit), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk unit.", Convert.ToInt32(uID), LoggerName, "");

                return unit;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }
    }
}
