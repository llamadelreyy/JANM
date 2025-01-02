using MySqlConnector;
using System.Data;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using System.Reflection;
using PBTPro.DAL.Services;
using PBTPro.DAL.Models.CommonServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PBTPro.Data
{
    public class RoleService : IDisposable
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
        private List<system_role> _Role { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        private readonly ILogger<RoleService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public RoleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateRole();
        }

        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        public async void CreateRole()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {

                //////var platformApiUrl = _configuration["PlatformAPI"];
                //////var accessToken = _cf.CheckToken();

                //////var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
                //////string jsonString = await _cf.List(request);

                ////////Open this when the API is completed
                //////_Role = JsonConvert.DeserializeObject<List<system_role>>(jsonString);
                ///

                _Role = new List<system_role> {
                        new system_role {
                            role_id = 1,
                            role_name = "Administrator",
                            role_desc = "Admin of the system",
                            created_date = DateTime.Parse("2023/03/11")
                        },
                        new system_role {
                            role_id = 2,
                            role_name = "Head of Department",
                            role_desc = "Head of department for perlesenan",
                            created_date = DateTime.Parse("2023/03/11")
                        }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan sistem.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }
            finally
            {
            }
        }

        public Task<List<system_role>> GetRoleAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Role);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<system_role>> InsertRole([FromBody] string strData = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Role/InsertRole");
                string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/Role/InsertRole");
                system_role dtData = JsonConvert.DeserializeObject<system_role>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah peranan baru.", Convert.ToInt32(uID), LoggerName, "");
                return dtData;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteRole(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Role/DeleteRole/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data peranan.", Convert.ToInt32(uID), LoggerName, "");

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
        public async Task<ActionResult<system_role>> UpdateRole(int id, system_role dtData)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Role/UpdateRole/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Role/UpdateRole/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk peranan.", Convert.ToInt32(uID), LoggerName, "");

                return dtData;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<system_role>> RefreshRoleAsync()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Role/ListRole");
                string jsonString = await _cf.List(request);
                List<system_role> dtData = JsonConvert.DeserializeObject<List<system_role>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan.", Convert.ToInt32(uID), LoggerName, "");

                return dtData;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

    }
}