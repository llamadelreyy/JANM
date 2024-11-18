/*
Project: PBT Pro
Description: Faq service to handle FAQ form field
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
using PBT.Pages;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;

namespace PBT.Data
{
    [AllowAnonymous]
    public partial class FaqService : IDisposable
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
        private List<FaqInfo> _Faq { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<FaqService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public FaqService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<FaqService> logger, PBTProDbContext dbContext)
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
        public async Task<List<FaqInfo>> GetAllFaq()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/ListFaq");
                string jsonString = await _cf.List(request);
                List<FaqInfo> faqList = JsonConvert.DeserializeObject<List<FaqInfo>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - GetAllFaq", "Papar semua senarai soalan lazim.", Convert.ToInt32(uID), LoggerName, "");

                return faqList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "FaqService - GetAllFaq", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<FaqInfo>> RefreshListFaq()
        {
            GetDefaultPermission();
            //var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/ListFaq");
                string jsonString = await _cf.List(request);

                List<FaqInfo> faqList = JsonConvert.DeserializeObject<List<FaqInfo>>(jsonString);
                 await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - RefreshListFaq", "Papar semula senarai soalan lazim.", 1, LoggerName, "");

                return faqList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error,  "FaqService - RefreshListFaq", ex.Message, 1, LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<FaqInfo> GetIdFaq(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/RetrieveFaq/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);
                FaqInfo faq = JsonConvert.DeserializeObject<FaqInfo>(jsonString.ToString());
                await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - GetIdFaq", "Papar maklumat terperinci soalan lazim.", Convert.ToInt32(uID), LoggerName, "");
                
                return faq;
            }
            catch (Exception ex) 
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "FaqService - GetIdFaq", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }        
                    
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<FaqInfo>> PostFaq([FromBody] string faqs="")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/InsertFaq");
                string jsonString = await _cf.AddNew(request, faqs, platformApiUrl + "/api/Faq/InsertFaq");
                FaqInfo faq = JsonConvert.DeserializeObject<FaqInfo>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - PostFaq", "Tambah data baru untuk soalan lazim.", Convert.ToInt32(uID), LoggerName, "");
                
                return faq;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "FaqService - PostFaq", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return faq;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteFaq(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/DeleteFaq/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - DeleteFaq", "Berjaya padam data untuk soalan lazim.", Convert.ToInt32(uID), LoggerName, "");

                return id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "FaqService - DeleteFaq", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult<FaqInfo>> PutFaq(int id, FaqInfo faq)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Faq/UpdateFaq/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Faq/UpdateFaq/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(faq), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, "FaqService - PutFaq", "Berjaya kemaskini data untuk soalan lazim.", Convert.ToInt32(uID), LoggerName, "");
                
                return faq;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "FaqService - PutFaq", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }
        public Task<List<FaqInfo>> GetFAQAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, "FaqService - GetFAQAsync", "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");

            return Task.FromResult(_Faq);
        }
    }
}
