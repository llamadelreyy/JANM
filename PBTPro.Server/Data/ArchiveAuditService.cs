/*
Project: PBT Pro
Description: Archive Audit service to handle audit list
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
using PBT.Pages;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;

namespace PBT.Data
{
    [AllowAnonymous]
    public partial class ArchiveAuditService : IDisposable
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
        private List<AuditlogArchiveInfo> _Audit { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<ArchiveAuditService> _logger;
        private string LoggerName = "";
        string _controllerName = "";
        public ArchiveAuditService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ArchiveAuditService> logger, PBTProDbContext dbContext)
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
        public async Task<List<AuditlogArchiveInfo>> GetAllAudit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Archive/ListAudit");
                string jsonString = await _cf.List(request);
                List<AuditlogArchiveInfo> auditList = JsonConvert.DeserializeObject<List<AuditlogArchiveInfo>>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, "ArchiveAuditService - GetAllAudit", "Papar semua senarai arkib log audit.", Convert.ToInt32(uID), LoggerName, "");
                return auditList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "ArchiveAuditService - GetAllAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<AuditlogArchiveInfo>> RefreshListAudit()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Archive/ListAudit");
                string jsonString = await _cf.List(request);
                List<AuditlogArchiveInfo> auditList = JsonConvert.DeserializeObject<List<AuditlogArchiveInfo>>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, "ArchiveAuditService - RefreshListAudit", "Papar semula semua senarai arkib log audit.", Convert.ToInt32(uID), LoggerName, "");
                return auditList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "ArchiveAuditService - RefreshListAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<AuditlogArchiveInfo> GetIdAudit(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Archive/RetrieveAudit/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);
                AuditlogArchiveInfo audit = JsonConvert.DeserializeObject<AuditlogArchiveInfo>(jsonString.ToString());

                await _cf.CreateAuditLog((int)AuditType.Information, "ArchiveAuditService - GetIdAudit", "Papar maklumat terperinci arkib audit log", Convert.ToInt32(uID), LoggerName, "");
                return audit;
            }
            catch (Exception ex) 
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "ArchiveAuditService - GetIdAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
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

                var request = _cf.CheckRequest(platformApiUrl + "/api/Archive/DeleteAudit/" + id);
                string jsonString = await _cf.Delete(request);

                await _cf.CreateAuditLog((int)AuditType.Information, "ArchiveAuditService - DeleteAudit", "Berjaya padam data untuk arkib log audit.", Convert.ToInt32(uID), LoggerName, "");
                return Ok(jsonString);
            }
            catch (Exception ex) 
            {
                await _cf.CreateAuditLog((int)AuditType.Error, "ArchiveAuditService - DeleteAudit", ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        private IActionResult Ok(string jsonString)
        {
            throw new NotImplementedException();
        }
        public Task<List<AuditlogArchiveInfo>> GetAuditAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, "ArchiveAuditService - GetAuditAsync", "Berjaya muat semula senarai untuk arkib log audit.", 1, LoggerName, "");
            return Task.FromResult(_Audit);
        }            
    }
}
