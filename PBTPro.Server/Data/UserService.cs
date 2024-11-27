using MySqlConnector;
using System.Data;
using System.Reflection;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using DevExpress.Map.Native;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PBTPro.Data
{
    public class UserService : IDisposable
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

        private List<UserProp> _User { get; set; }
        public IConfiguration _configuration { get; }
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<UserService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateUserList();
        }

        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        public async void CreateUserList()
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
                //////_User = JsonConvert.DeserializeObject<List<UserProp>>(jsonString);

                _User = new List<UserProp> {
                    new UserProp {
                        user_id = 1,
                        user_name = "mbdk240015",
                        full_name = "Azman Bin Alias",
                        created_date = DateTime.Parse("2024/01/05")
                    },
                    new UserProp {
                        user_id = 2,
                        user_name = "mbdk230010",
                        full_name = "Abu Bakar Bin Jamal",
                        created_date = DateTime.Parse("2023/03/10")
                    }
                 };


                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna sistem.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }
        }

        public Task<List<UserProp>> GetUserAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_User);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserProp>> InsertUser([FromBody] string strData = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/User/InsertUser");
                string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/User/InsertUser");
                UserProp dtData = JsonConvert.DeserializeObject<UserProp>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah pengguna baru.", Convert.ToInt32(uID), LoggerName, "");
                return dtData;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteUser(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/User/DeleteUser/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data pengguna.", Convert.ToInt32(uID), LoggerName, "");

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
        public async Task<ActionResult<UserProp>> UpdateUser(int id, UserProp dtData)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/User/UpdateUser/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/User/UpdateUser/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data pengguna.", Convert.ToInt32(uID), LoggerName, "");

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
        public async Task<List<UserProp>> RefreshUserAsync()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
                string jsonString = await _cf.List(request);
                List<UserProp> dtData = JsonConvert.DeserializeObject<List<UserProp>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna.", Convert.ToInt32(uID), LoggerName, "");

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