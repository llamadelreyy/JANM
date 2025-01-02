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

        private List<system_user> _User { get; set; }
        public IConfiguration _configuration { get; }
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        private readonly ILogger<UserService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
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
                //////_User = JsonConvert.DeserializeObject<List<system_user>>(jsonString);

                _User = new List<system_user> {
                    new system_user {
                        user_id = 1,
                        user_name = "650721031221",
                        full_name = "Azman Bin Alias",
                        ic_no = "650721031221",
                        mobile_no = "0123456789",
                        user_email = "john_doe@gmail.com",
                        dept_id = 1,
                        dept_name = "Jabatan Penilaian",
                        section_id = 1,
                        section_name =  "Harta dan Pusaka",
                        unit_id = 1,
                        unit_name = "Unit 001",
                        created_date = DateTime.Parse("2024/01/05")
                    },
                    new system_user {
                        user_id = 2,
                        user_name = "701121163223",
                        full_name = "Abu Bakar Bin Jamal",
                        ic_no = "701121163223",
                        mobile_no = "0129876543",
                        user_email = "azman610@gmail.com",
                        dept_id = 1,
                        dept_name = "Jabatan Penilaian",
                        section_id = 1,
                        section_name =  "Harta dan Pusaka",
                        unit_id = 1,
                        unit_name = "Unit 001",
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

        public Task<List<system_user>> GetUserAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_User);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<int> AddUser([FromBody] string strData = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/User/InsertUser");
                string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/User/InsertUser");
                system_user dtData = JsonConvert.DeserializeObject<system_user>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah pengguna baru.", Convert.ToInt32(uID), LoggerName, "");
                return dtData.user_id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [HttpDelete]
        public async Task<bool> DeleteUser(int id)
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

                return true;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<int> UpdateUser(system_user dtData)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data pengguna.", Convert.ToInt32(uID), LoggerName, "");

                return dtData.user_id;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<system_user>> RefreshUserAsync()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
                string jsonString = await _cf.List(request);
                List<system_user> dtData = JsonConvert.DeserializeObject<List<system_user>>(jsonString);
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