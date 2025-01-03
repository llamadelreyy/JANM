using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Text;

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

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<UnitService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Unit";
        private string LoggerName = "";
        private List<ref_unit> _Unit { get; set; }

        public UnitService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UnitService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        }
        [HttpGet]
        public async Task<List<ref_unit>> ListAll()
        {
            var result = new List<ref_unit>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_unit>>(dataString);
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
                result = new List<ref_unit>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<ref_unit>> Refresh()
        {
            var result = new List<ref_unit>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_unit>>(dataString);
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
                result = new List<ref_unit>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(ref_unit inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, ref_unit inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.div_id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk jadual rondaan.", 1, LoggerName, "");

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
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public Task<List<ref_unit>> GetUnitAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");
            return Task.FromResult(_Unit);
        }

        public async Task<ref_unit> ViewDetail(int id)
        {
            var result = new ref_unit();
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
                        result = JsonConvert.DeserializeObject<ref_unit>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new ref_unit();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
    }
}
//        public IConfiguration _configuration { get; }
//        private List<unit_info> _Unit { get; set; }

//        private readonly PBTProDbContext _dbContext;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        protected readonly CommonFunction _cf;
//        protected readonly SharedFunction _sf;
//        private readonly ILogger<UnitService> _logger;
//        private string LoggerName = "";
//        string _controllerName = "";

//        public UnitService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UnitService> logger, PBTProDbContext dbContext)
//        {
//            _configuration = configuration;
//            _dbContext = dbContext;
//            _httpContextAccessor = httpContextAccessor;
//            _cf = new CommonFunction(httpContextAccessor, configuration);
//            _sf = new SharedFunction(httpContextAccessor);
//            _logger = logger;
//            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
//        }
//        public void GetDefaultPermission()
//        {
//            if (LoggerName != null || LoggerName != "")
//                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
//            else LoggerName = null;
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<List<unit_info>> GetAllUnit()
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/ListUnit");
//                string jsonString = string.Empty;
//                if (request.Content != null)
//                    jsonString = await _cf.List(request);

//                List<unit_info> unitList = JsonConvert.DeserializeObject<List<unit_info>>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai unit.", Convert.ToInt32(uID), LoggerName, "");

//                //FOR TESTING PURPOSE ===============
//                unitList = new List<unit_info> {
//                    new unit_info {
//                        unit_id = 1,
//                        dept_id = 1,
//                        section_id = 1,
//                        dept_code = "001",
//                        dept_name = "Jabatan Penilaian",
//                        section_code = "001",
//                        section_name = "Harta dan Pusaka",
//                        unit_code = "001",
//                        unit_name = "Unit 001",
//                        unit_desc = "Unit pengurusan harta pusaka",
//                        created_date = DateTime.Parse("2024/01/05")
//                    }
//                 };
//                return unitList;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<List<unit_info>> RefreshUnit()
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/ListUnit");
//                string jsonString = await _cf.List(request);
//                List<unit_info> unitList = JsonConvert.DeserializeObject<List<unit_info>>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai unit.", Convert.ToInt32(uID), LoggerName, "");

//                return unitList;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }


//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<int> AddUnit([FromBody] string units = "")
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/InsertUnit");
//                string jsonString = await _cf.AddNew(request, units, platformApiUrl + "/api/Unit/InsertUnit");
//                unit_info unitList = JsonConvert.DeserializeObject<unit_info>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah data baru untuk unit.", Convert.ToInt32(uID), LoggerName, "");

//                return unitList.dept_id;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return 0;
//            }
//        }

//        [HttpDelete]
//        public async Task<bool> DeleteUnit(int id)
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Unit/DeleteUnit/" + id);
//                string jsonString = await _cf.Delete(request);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk unit.", Convert.ToInt32(uID), LoggerName, "");

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return false;
//            }
//        }

//        [AllowAnonymous]
//        [HttpPut]
//        public async Task<int> UpdateUnit(unit_info unit)
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var uri = platformApiUrl + "/api/Unit/UpdateUnit/" + unit.unit_id;
//                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Unit/UpdateUnit/" + unit.unit_id);
//                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(unit), uri);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk unit.", Convert.ToInt32(uID), LoggerName, "");

//                return unit.unit_id;

//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return 0;
//            }
//        }
//    }
//}
