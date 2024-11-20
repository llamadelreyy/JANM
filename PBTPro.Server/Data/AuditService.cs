/*
Project: PBT Pro
Description: Audit service to handle audit list
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
using PBTPro.Pages;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;

namespace PBTPro.Data
{
    [AllowAnonymous]
    public partial class AuditService : IDisposable
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
        private List<auditlog_info> _Audit { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<AuditService> _logger;
        private string LoggerName = "";
        string _controllerName = "";
        public AuditService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuditService> logger, PBTProDbContext dbContext)
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
        public async Task<List<auditlog_info>> GetAllAudit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/ListAudit");
                string jsonString = await _cf.List(request);
                List<auditlog_info> auditList = JsonConvert.DeserializeObject<List<auditlog_info>>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - GetAllAudit", "Papar semua senarai log audit.", Convert.ToInt32(uID), LoggerName, "");
                return auditList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - GetAllAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<auditlog_info>> RefreshListAudit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/ListAudit");
                string jsonString = await _cf.List(request);
                List<auditlog_info> auditList = JsonConvert.DeserializeObject<List<auditlog_info>>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - RefreshListAudit", "Papar semula semua senarai log audit.", Convert.ToInt32(uID), LoggerName, "");
                return auditList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - RefreshListAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<auditlog_info> GetIdAudit(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/RetrieveAudit/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);
                auditlog_info audit = JsonConvert.DeserializeObject<auditlog_info>(jsonString.ToString());

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - GetIdAudit", "Papar maklumat terperinci c.", Convert.ToInt32(uID), LoggerName, "");
                return audit;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - GetIdAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpPost]
        public async Task<auditlog_info> AddNewAudit(string strAudit)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/InsertAudit");
                string jsonString = await _cf.AddNew(request, strAudit, platformApiUrl + "/api/Audit/InsertAudit");
                auditlog_info audit = JsonConvert.DeserializeObject<auditlog_info>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - AddNewAudit", "Berjaya tambah data untuk log audit.", Convert.ToInt32(uID), LoggerName, "");
                return audit;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - AddNewAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/DeleteAudit/" + id);
                string jsonString = await _cf.Delete(request);

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - DeleteAudit", "Berjaya padam data untuk log audit.", Convert.ToInt32(uID), LoggerName, "");
                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - DeleteAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        private IActionResult Ok(string jsonString)
        {
            throw new NotImplementedException();
        }
        public Task<List<auditlog_info>> GetAuditAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, "AuditService - GetAuditAsync", "Berjaya muat semula senarai untuk log audit.", 1, LoggerName, "");
            return Task.FromResult(_Audit);
        }

        [HttpGet]
        public async Task<IActionResult> InsertArchiveAudit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/InsertArchiveAudit/");
                string jsonString = await _cf.List(request);

                await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - ArchiveAuditLog", "Berjaya arkib data untuk log audit.", 1, LoggerName, "");
                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - ArchiveAuditLog", ex.Message, 1, LoggerName, "");
                return null;
            }
        }
    }
}
