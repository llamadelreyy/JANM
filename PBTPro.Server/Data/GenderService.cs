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
    public class GenderService : IDisposable
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
        private readonly ILogger<GenderService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Gender";
        private string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

        private List<ref_gender> _Gender { get; set; }

        public GenderService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<GenderService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
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
        public async Task<List<ref_gender>> ListAll()
        {
            var result = new List<ref_gender>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_gender>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai data.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<ref_gender>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<ref_gender>> Refresh()
        {
            var result = new List<ref_gender>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ref_gender>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai data.", LoggerID, LoggerName, GetType().Name, RoleID); ;
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<ref_gender>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(ref_gender inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data.", LoggerID, LoggerName, GetType().Name, RoleID);
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, ref_gender inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.gen_id}";
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

        public Task<List<ref_gender>> GetRaceAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai data.", LoggerID, LoggerName, GetType().Name, RoleID);
            return Task.FromResult(_Gender);
        }

        public async Task<ref_gender> ViewDetail(int id)
        {
            var result = new ref_gender();
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
                        result = JsonConvert.DeserializeObject<ref_gender>(dataString);
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
                result = new ref_gender();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }
    }
}