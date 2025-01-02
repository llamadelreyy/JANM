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
    public class UserProfileService : IDisposable
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
        private List<user_profile> _Profile { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<UserProfileService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public UserProfileService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserProfileService> logger, PBTProDbContext dbContext)
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
        public async Task<List<user_profile>> Retrieve(string userId)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Profile/Retrieve/" + userId);
                string jsonString = string.Empty;
                if (request.Content != null)
                    jsonString = await _cf.List(request);

                List<user_profile> userProfile = JsonConvert.DeserializeObject<List<user_profile>>(jsonString);
                //for testing -->  await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Capaian profail pengguna.", Convert.ToInt32(uID), LoggerName, "");

                //FOR TESTING PURPOSE ===============
                userProfile = new List<user_profile> {
                    new user_profile {
                        profile_department_id = 1,
                        profile_department_name = "Jabatan Penilaian",
                        profile_section_id = 1,
                        profile_section_name = "Harta dan Pusaka",
                        profile_unit_id = 1,
                        profile_unit_name = "Unit 001", 
                        profile_role_id = 1,
                        profile_role = "Penguatkuasa",
                        profile_user_id = "750727085221",
                        profile_name = "John Doe",
                        profile_icno = "750727085221",
                        profile_email = "john_doe@gmail.com",
                        profile_tel_no =  "0123678902",
                        created_date = DateTime.Parse("2024/01/05")
                    }
                 };
                //====================================

                return userProfile;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<bool> UpdateProfile(int userId, user_profile profile)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Profile/UpdateProfile/" + userId;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Profile/UpdateProfile/" + userId);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(profile), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini profail pengguna.", Convert.ToInt32(uID), LoggerName, "");

                return true;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return false;
            }
        }
    }
}
