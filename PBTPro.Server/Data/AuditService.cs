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
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;

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
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<AuditService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Audit";
        private string LoggerName = "";

        private List<auditlog_info> _Audit { get; set; }
        public AuditService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuditService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cf = new AuditLogger(configuration, apiConnector);
            _logger = logger;
            _dbContext = dbContext;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
        }
        public Task<List<auditlog_info>> GetAuditAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk log audit.", 1, "", LoggerName);
            return Task.FromResult(_Audit);
        }

        [HttpGet]
        public async Task<List<auditlog_info>> GetAllAudit()
        {            
            var result = new List<auditlog_info>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAudit";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<auditlog_info>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai log audit. " + response.ReturnCode, 1, LoggerName, "");
                    }
                }
                else {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod:" + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<auditlog_info>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<auditlog_info>> RefreshListAudit()
        {
            var result = new List<auditlog_info>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAudit";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<auditlog_info>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai log audit.", 1, LoggerName, "");
                    }
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod:" + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<auditlog_info>();
            }
            return result;
        }

        public async Task<auditlog_info> ViewDetail(int id)
        {
            var result = new auditlog_info();
            try
            {
                string requestquery = $"/{id}";
                string requestUrl = $"{_baseReqURL}/RetrieveAudit{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<auditlog_info>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new auditlog_info();
            }

            return result;
        }
 
        public async Task<ReturnViewModel> DeleteAudit(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/DeleteAudit/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk jadual rondaan.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        [HttpGet]
        public async Task<List<auditlog_info>> InsertArchiveAudit()
        {
            var result = new List<auditlog_info>();
            try
            {
                string requestUrl = $"{_baseReqURL}/InsertArchiveAudit";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<auditlog_info>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya arkib data untuk log audit.", 1, LoggerName, "");
                    }
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod:" + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<auditlog_info>();
            }
            return result;
        }

        //public async Task<ReturnViewModel> AddNewAudit(auditlog_info inputModel)
        //{
        //    var result = new ReturnViewModel();
        //    try
        //    {
        //        var reqData = JsonConvert.SerializeObject(inputModel);
        //        var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

        //        string requestUrl = $"{_baseReqURL}/InsertAudit";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

        //        result = response;
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk log audit.", 1, LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new ReturnViewModel();
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //    return result;
        //}
        //[HttpPost]
        //public async Task<auditlog_info> AddNewAudit(string strAudit)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/InsertAudit");
        //        string jsonString = await _cf.AddNew(request, strAudit, platformApiUrl + "/api/Audit/InsertAudit");
        //        auditlog_info audit = JsonConvert.DeserializeObject<auditlog_info>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk log audit.", 1, LoggerName, "");
        //        return audit;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}
        //[HttpDelete]
        //public async Task<IActionResult> DeleteAudit(int id)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/DeleteAudit/" + id);
        //        string jsonString = await _cf.Delete(request);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk log audit.", 1, LoggerName, "");
        //        return Ok(jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<auditlog_info> GetIdAudit(int id)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/RetrieveAudit/" + id);
        //        string jsonString = await _cf.Retrieve(request, accessToken);
        //        auditlog_info audit = JsonConvert.DeserializeObject<auditlog_info>(jsonString.ToString());
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci c.", 1, LoggerName, "");
        //        return audit;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}        
        //[HttpGet]
        //public async Task<IActionResult> InsertArchiveAudit()
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Audit/InsertArchiveAudit/");
        //        string jsonString = await _cf.List(request);

        //        await _cf.CreateAuditLog((int)AuditType.Information, "AuditService - ArchiveAuditLog", "Berjaya arkib data untuk log audit.", 1, LoggerName, "");
        //        return Ok(jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, "AuditService - ArchiveAuditLog", ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}
    }
}
