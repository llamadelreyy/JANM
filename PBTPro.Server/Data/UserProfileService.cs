using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

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
        //public IConfiguration _configuration { get; }
        //private List<user_profile> _Profile { get; set; }

        //private readonly PBTProDbContext _dbContext;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //protected readonly CommonFunction _cf;
        //private readonly ILogger<UserProfileService> _logger;
        //private string LoggerName = "";
        //string _controllerName = "";

        //public UserProfileService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserProfileService> logger, PBTProDbContext dbContext)
        //{
        //    _configuration = configuration;
        //    _dbContext = dbContext;
        //    _httpContextAccessor = httpContextAccessor;
        //    _cf = new CommonFunction(httpContextAccessor, configuration);
        //    _logger = logger;
        //    _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        //}

        public IConfiguration _configuration { get; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<UserProfileService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/User";
        private string LoggerName = "";
        //private List<user_profile_view> _Profile { get; set; }

        public UserProfileService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserProfileService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            LoggerName = _PBTAuthStateProvider.CurrentUser.Fullname;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<List<user_profile>> Retrieve(string userId)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Profile/Retrieve/" + userId);
        //        string jsonString = string.Empty;
        //        if (request.Content != null)
        //            jsonString = await _cf.List(request);

        //        List<user_profile> userProfile = JsonConvert.DeserializeObject<List<user_profile>>(jsonString);
        //        //for testing -->  await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Capaian profail pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //        //FOR TESTING PURPOSE ===============
        //        userProfile = new List<user_profile> {
        //            new user_profile {
        //                profile_department_id = 1,
        //                profile_department_name = "Jabatan Penilaian",
        //                profile_section_id = 1,
        //                profile_section_name = "Harta dan Pusaka",
        //                profile_unit_id = 1,
        //                profile_unit_name = "Unit 001", 
        //                profile_role_id = 1,
        //                profile_role = "Penguatkuasa",
        //                profile_user_id = "750727085221",
        //                profile_name = "John Doe",
        //                profile_icno = "750727085221",
        //                profile_email = "john_doe@gmail.com",
        //                profile_tel_no =  "0123678902",
        //                created_date = DateTime.Parse("2024/01/05")
        //            }
        //         };
        //        //====================================

        //        return userProfile;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}


        public async Task<user_profile_view> Retrieve()
        {
            //var result = new List<user_profile_view>();
            var result = new user_profile_view();
            string requestUrl = $"{_baseReqURL}/GetProfile";            
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<user_profile_view>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new user_profile_view();
            }
            return result;
        }

        //public async Task<List<user_profile>> Retrieve(int id)
        //{
        //    var result = new user_profile();
        //    try
        //    {
        //        string requestquery = $"/{id}";
        //        string requestUrl = $"{_baseReqURL}/ViewDetail{requestquery}";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl);

        //        if (response.ReturnCode == 200)
        //        {
        //            string? dataString = response?.Data?.ToString();
        //            if (!string.IsNullOrWhiteSpace(dataString))
        //            {
        //                result = JsonConvert.DeserializeObject<user_profile>(dataString);
        //            }
        //            await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci soalan lazim.", 1, LoggerName, "");
        //        }
        //        else
        //        {
        //            await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new user_profile();
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //    return result;
        //}

        public async Task<ReturnViewModel> UpdateProfile(update_profile_input_model inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/UpdateProfile";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini profil pengguna.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "TIdak berjaya kemaskini profil pengguna.", 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> UpdatePassword(update_password_input_model inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/UpdatePassword";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini katalaluan pengguna.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "TIdak berjaya kemaskini katalaluan pengguna.", 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> UpdateAvatar(update_avatar_input_model inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                //var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new MultipartFormDataContent(); //new StringContent(reqData, Encoding.UTF8, "application/json");
                reqContent.Add(new StringContent(inputModel.user_id.ToString()), "user_id");
                var streamContent = new StreamContent(inputModel.avatar_image.OpenReadStream());
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "avatar_image",
                    FileName = inputModel.avatar_image.FileName
                };
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(inputModel.avatar_image.ContentType);
                reqContent.Add(streamContent);
                string requestUrl = $"{_baseReqURL}/UpdateAvatar";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat naik data untuk seksyen.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                result = response;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new ReturnViewModel();
            }

            return result;
        }
    


        //        [AllowAnonymous]
        //        [HttpPut]
        //        public async Task<bool> UpdateProfile(int userId, user_profile profile)
        //        {
        //1            try
        //            {
        //                var platformApiUrl = _configuration["PlatformAPI"];
        //                var accessToken = _cf.CheckToken();

        //                var uri = platformApiUrl + "/api/Profile/UpdateProfile/" + userId;
        //                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Profile/UpdateProfile/" + userId);
        //                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(profile), uri);
        //                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini profail pengguna.", 1, LoggerName, "");

        //                return true;

        //            }
        //            catch (Exception ex)
        //            {
        //                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message,1, LoggerName, "");
        //                return false;
        //            }
        //        }
    }
}
