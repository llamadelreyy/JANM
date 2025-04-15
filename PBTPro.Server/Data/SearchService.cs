using Microsoft.AspNetCore.Mvc;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using PBTPro.DAL;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using System.Reflection;
using System.Text;
using PBTPro.DAL.Models.PayLoads;

namespace PBTPro.Data
{
    public class SearchService : IDisposable
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
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        protected readonly AuditLogger _cf;
        private string _baseReqURL = "/api/GeneralSearch";
        public string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

        private List<view_premis_detail> _PremisSearch { get; set; }

        public SearchService(IConfiguration configuration, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
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

        public async Task<List<general_search_premis_detail>> GetListPremisDetails(int? maxResult = null, int? page = 1)
        {
            var result = new List<general_search_premis_detail>();

            try
            {
                string requestquery = "";

                if (maxResult != null)
                    requestquery = $"&maxResult={maxResult}";


                string requestUrl = $"{_baseReqURL}/GetListPremisDetails?page={page}{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<general_search_premis_detail>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Carian maklumat premis.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat! Status Kod : " + response.ReturnCode + " " + response.ReturnMessage, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<general_search_premis_detail>();
            }
            return result;
        }

    }
}