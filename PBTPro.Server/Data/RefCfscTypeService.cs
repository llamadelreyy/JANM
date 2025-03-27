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
    public class RefCfscTypeService : IDisposable
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
        private readonly ILogger<RefCfscTypeService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        private string _baseReqURL = "/api/ConfiscationItem";
        private string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

        public RefCfscTypeService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RefCfscTypeService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            LoggerName = _PBTAuthStateProvider.CurrentUser.Fullname;
            LoggerID = _PBTAuthStateProvider.CurrentUser.Userid;
            RoleID = _PBTAuthStateProvider.CurrentUser.Roleid;
        }

        [HttpGet]
        public async Task<List<ref_cfsc_invtype>> ListAll()
        {
            var result = new List<ref_cfsc_invtype>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_cfsc_invtype>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai jenis barang sitaan.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<ref_cfsc_invtype>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<ref_cfsc_invtype>> Refresh()
        {
            var result = new List<ref_cfsc_invtype>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_cfsc_invtype>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai jenis barang sitaan.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode + response.ReturnMessage, LoggerID, LoggerName, GetType().Name, RoleID);
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<ref_cfsc_invtype>();
            }
            return result;
        }

        [HttpPost]
        public async Task<ReturnViewModel> Add(ref_cfsc_invtype inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                //ApplicationRole rm = new ApplicationRole();
                //rm.Name = inputModel.Name;
                //rm.RoleDesc = inputModel.RoleDesc;

                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat! Status kod : " + response.ReturnCode + " - " + response.ReturnMessage, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (HttpRequestException ex)
            {
                result = new ReturnViewModel
                {
                    ReturnCode = 500,
                    ReturnMessage = $"Request failed: {ex.Message}"
                };
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, $"Request failed: {ex.Message}", LoggerID, LoggerName, GetType().Name, RoleID);
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, ref_cfsc_invtype inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.inv_type_id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data.", LoggerID, LoggerName, GetType().Name, RoleID);

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
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
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data.", LoggerID, LoggerName, GetType().Name, RoleID);
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }

        public async Task<ref_cfsc_invtype> ViewDetail(int id)
        {
            var result = new ref_cfsc_invtype();
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
                        result = JsonConvert.DeserializeObject<ref_cfsc_invtype>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                result = new ref_cfsc_invtype();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }
    }
}
