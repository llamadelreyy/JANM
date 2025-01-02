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
    public class SectionService : IDisposable
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
        private readonly ILogger<SectionService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Section";
        private string LoggerName = "";
        private List<section_info> _Section { get; set; }

        public SectionService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<SectionService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        }
        [HttpGet]
        public async Task<List<section_info>> ListAll()
        {
            var result = new List<section_info>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<section_info>>(dataString);
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
                result = new List<section_info>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<section_info>> Refresh()
        {
            var result = new List<section_info>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<section_info>>(dataString);
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
                result = new List<section_info>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(section_info inputModel)
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

        public async Task<ReturnViewModel> Update(int id, section_info inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.section_id}";
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

        public Task<List<section_info>> GetSectionAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");
            return Task.FromResult(_Section);
        }

        public async Task<section_info> ViewDetail(int id)
        {
            var result = new section_info();
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
                        result = JsonConvert.DeserializeObject<section_info>(dataString);
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
                result = new section_info();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
    }
}

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<List<section_info>> GetAllSection()
//        {
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Section/ListSection");
//                string jsonString = string.Empty;
//                if (request.Content != null)
//                    jsonString = await _cf.List(request);


//                List<section_info> sectionList = JsonConvert.DeserializeObject<List<section_info>>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai seksyen.", Convert.ToInt32(uID), LoggerName, "");

//                //FOR TESTING PURPOSE ===============
//                sectionList = new List<section_info> {
//                    new section_info {
//                        section_id = 1,
//                        dept_id = 1,
//                        dept_code = "001",
//                        dept_name = "Jabatan Penilaian",
//                        section_code = "001",
//                        section_name = "Harta dan Pusaka",
//                        section_desc = "Penilaian Harta dan pusaka",
//                        created_date = DateTime.Parse("2024/01/05")
//                    }
//                 };
//                //====================================

//                return sectionList;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<List<section_info>> RefreshSection()
//        {
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Section/ListSection");
//                string jsonString = await _cf.List(request);
//                List<section_info> sectionList = JsonConvert.DeserializeObject<List<section_info>>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai seksyen.", Convert.ToInt32(uID), LoggerName, "");

//                return sectionList;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }


//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<int> AddSection([FromBody] string sections = "")
//        {
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Section/InsertSection");
//                string jsonString = await _cf.AddNew(request, sections, platformApiUrl + "/api/Section/InsertSection");
//                section_info sectionList = JsonConvert.DeserializeObject<section_info>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah data baru untuk section.", Convert.ToInt32(uID), LoggerName, "");

//                return sectionList.section_id;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return 0;
//            }
//        }

//        [HttpDelete]
//        public async Task<bool> DeleteSection(int id)
//        {
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/Section/DeleteSection/" + id);
//                string jsonString = await _cf.Delete(request);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk seksyen.", Convert.ToInt32(uID), LoggerName, "");

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
//        public async Task<int> UpdateSection(section_info section)
//        {
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var uri = platformApiUrl + "/api/Section/UpdateSection/" + section.section_id;
//                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Section/UpdateSection/" + section.section_id);
//                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(section), uri);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk seksyen.", Convert.ToInt32(uID), LoggerName, "");

//                return section.section_id;

//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return 0;
//            }
//        }
//    }
//}
