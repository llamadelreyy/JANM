/*
Project: PBT Pro
Description: Shared service to handle menu
Author: Ismail
Date: Jan 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
09/01/2025 - initial create
*/
using Newtonsoft.Json;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Text;

namespace PBTPro.Data
{
    public partial class PermissionService : IDisposable
    {
        public IConfiguration _configuration { get; }
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        protected readonly AuditLogger _cf;
        private string _baseReqURL = "/api/Permission";
        private bool disposed = false;
        private string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

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

        public PermissionService(IConfiguration configuration, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
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

        public async Task<List<permission>> ListAll()
        {
            var result = new List<permission>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetList";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<permission>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<permission>();
            }

            return result;
        }

        public async Task<List<permission>> GetListByRole(int role_id)
        {
            var result = new List<permission>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetListByRole?roleId={role_id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<permission>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai kebenaran berdasarkan peranan (" + role_id + ").", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<permission>();
            }

            return result;
        }
        
        public async Task<List<permission>> GetListByMenu(int menu_id)
        {
            var result = new List<permission>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetListByMenu?menuId={menu_id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<permission>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan berdasarkan menu (" + menu_id + ").", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<permission>();
            }

            return result;
        }

        public async Task<permission> ViewDetail(int id)
        {
            var result = new permission();
            try
            {
                string requestquery = $"/{id}";
                string requestUrl = $"{_baseReqURL}/GetDetail{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<permission>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new permission();
            }

            return result;
        }

        public async Task<ReturnViewModel> Add(permission inputModel)
        {

            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new ReturnViewModel();
            }

            return result;
        }

        public async Task<ReturnViewModel> Update(permission inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel.permission_id;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
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
                string requestUrl = $"{_baseReqURL}/Remove/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new ReturnViewModel();
            }

            return result;
        }

        public async Task<ReturnViewModel> BulkSaveByMenu(List<permission> inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel[0].menu_id;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/BulkSaveByMenu/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }

            return result;
        }

        public async Task<ReturnViewModel> BulkSaveByRole(List<permission> inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel[0].role_id;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/BulkSaveByRole/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }

            return result;
        }

        public async Task<ReturnViewModel> BulkSaveDynamic(List<permission> inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel[0].role_id;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/BulkSaveDynamic";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk kebenaran.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }

            return result;
        }
    }
}
