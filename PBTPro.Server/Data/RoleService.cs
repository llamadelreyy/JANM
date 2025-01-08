using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using static System.Net.WebRequestMethods;

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

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<RoleService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        //protected readonly CommonFunction _af;
        private string _baseReqURL = "/api/Role";
        private string LoggerName = "";
        //private List<role> _Role { get; set; }

        public RoleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            //_af = new CommonFunction(_httpContextAccessor, configuration);
        }


        [HttpGet]
        public async Task<List<ApplicationRole>> TestRole()
        {
            var result = new List<ApplicationRole>();
            string requestUrl = $"{_baseReqURL}/GetRoles";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ApplicationRole>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai peranan.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<ApplicationRole>();
            }
            return result;
        }


        [HttpGet]
        public async Task<List<ApplicationRole>> ListAll()
        {
            var result = new List<ApplicationRole>();
            string requestUrl = $"{_baseReqURL}/GetList";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ApplicationRole>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai peranan.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //var requestA = _af.CheckRequest(requestUrl);
                //string jsonString = await _af.List(requestA);

                //List<ApplicationRole> cities = JsonConvert.DeserializeObject<List<ApplicationRole>>(jsonString);
               // return cities;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<ApplicationRole>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<ApplicationRole>> Refresh()
        {
            var result = new List<ApplicationRole>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetList";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ApplicationRole>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai peranan.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode + response.ReturnMessage, 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<ApplicationRole>();
            }
            return result;
        }

        [HttpPost]
        public async Task<ReturnViewModel> Add(ApplicationRole inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat! Status kod : " + response.ReturnCode + " - " + response.ReturnMessage, 1, LoggerName, "");
                }                
            }
            catch (HttpRequestException ex)
            {
                result = new ReturnViewModel
                {
                    ReturnCode = 500,
                    ReturnMessage = $"Request failed: {ex.Message}"
                };
                await _cf.CreateAuditLog((int)AuditType.Error,GetType().Name + " - " + MethodBase.GetCurrentMethod().Name,$"Request failed: {ex.Message}",1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
        
        public async Task<ReturnViewModel> Update(int id, ApplicationRole inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.Id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data.", 1, LoggerName, "");

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Delete(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/Delete/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        //public Task<List<role>> GetRoleAsync(CancellationToken ct = default)
        //{
        //    var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai peranan.", 1, LoggerName, "");
        //    return Task.FromResult(_Role);
        //}

        public async Task<ApplicationRole> ViewDetail(int id)
        {
            var result = new ApplicationRole();
            try
            {
                string requestquery = $"/{id}";
                string requestUrl = $"{_baseReqURL}/ViewDetail{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<ApplicationRole>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new ApplicationRole();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        #region
        //public IConfiguration _configuration { get; }
        //private List<system_role> _Role { get; set; }

        //private readonly PBTProDbContext _dbContext;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //protected readonly CommonFunction _cf;
        //private readonly ILogger<RoleService> _logger;
        //private string LoggerName = "";
        //string _controllerName = "";

        //public RoleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger, PBTProDbContext dbContext)
        //{
        //    _configuration = configuration;
        //    _dbContext = dbContext;
        //    _httpContextAccessor = httpContextAccessor;
        //    _cf = new CommonFunction(httpContextAccessor, configuration);
        //    _logger = logger;
        //    _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        //    CreateRole();
        //}

        //public void GetDefaultPermission()
        //{
        //    if (LoggerName != null || LoggerName != "")
        //        LoggerName = "1";//User.Identity.Name;  // assign value to logger name
        //    else LoggerName = null;
        //}

        //public async void CreateRole()
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

        //    try
        //    {

        //        //////var platformApiUrl = _configuration["PlatformAPI"];
        //        //////var accessToken = _cf.CheckToken();

        //        //////var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
        //        //////string jsonString = await _cf.List(request);

        //        ////////Open this when the API is completed
        //        //////_Role = JsonConvert.DeserializeObject<List<system_role>>(jsonString);
        //        ///

        //        _Role = new List<system_role> {
        //                new system_role {
        //                    role_id = 1,
        //                    role_name = "Administrator",
        //                    role_desc = "Admin of the system",
        //                    created_date = DateTime.Parse("2023/03/11")
        //                },
        //                new system_role {
        //                    role_id = 2,
        //                    role_name = "Head of Department",
        //                    role_desc = "Head of department for perlesenan",
        //                    created_date = DateTime.Parse("2023/03/11")
        //                }
        //         };

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan sistem.", Convert.ToInt32(uID), LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //    }
        //    finally
        //    {
        //    }
        //}

        //public Task<List<system_role>> GetRoleAsync(CancellationToken ct = default)
        //{
        //    return Task.FromResult(_Role);
        //}


        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult<system_role>> InsertRole([FromBody] string strData = "")
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Role/InsertRole");
        //        string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/Role/InsertRole");
        //        system_role dtData = JsonConvert.DeserializeObject<system_role>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah peranan baru.", Convert.ToInt32(uID), LoggerName, "");
        //        return dtData;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return null;
        //    }
        //}

        //[HttpDelete]
        //public async Task<int> DeleteRole(int id)
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Role/DeleteRole/" + id);
        //        string jsonString = await _cf.Delete(request);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data peranan.", Convert.ToInt32(uID), LoggerName, "");

        //        return id;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return 0;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPut]
        //public async Task<ActionResult<system_role>> UpdateRole(int id, system_role dtData)
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var uri = platformApiUrl + "/api/Role/UpdateRole/" + id;
        //        var request = _cf.CheckRequestPut(platformApiUrl + "/api/Role/UpdateRole/" + id);
        //        string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk peranan.", Convert.ToInt32(uID), LoggerName, "");

        //        return dtData;

        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return null;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<List<system_role>> RefreshRoleAsync()
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Role/ListRole");
        //        string jsonString = await _cf.List(request);
        //        List<system_role> dtData = JsonConvert.DeserializeObject<List<system_role>>(jsonString);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan.", Convert.ToInt32(uID), LoggerName, "");

        //        return dtData;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return null;
        //    }
        //}
        #endregion
    }
}