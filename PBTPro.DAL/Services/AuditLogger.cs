using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Text;

namespace PBTPro.DAL.Services
{
    public class AuditLogger : ControllerBase, IDisposable
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
        
        private readonly ILogger<AuditLogger> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Audit";
        private string LoggerName = "";
        protected readonly string? _dbConn;

        private List<auditlog_info> _Audit { get; set; }
        public IConfiguration Configuration { get; }

        public AuditLogger(IConfiguration configuration, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");            
            _apiConnector = apiConnector;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
        }
        public async Task<ReturnViewModel> CreateAuditLog(int intType, string strMethod, string strMessage, int userId, string uname, string moduleName, int roleid = 0)
        {
            var result = new ReturnViewModel();
            try
            {
                auditlog_info inputModel = new auditlog_info()
                {
                    role_id = roleid,
                    module_name = string.IsNullOrEmpty(moduleName) ? "NA" : moduleName,
                    log_descr = strMessage,
                    creator_id = userId,
                    log_type = intType,
                    username = uname,
                    log_method = strMethod
                };

                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;

                await Task.Delay(500); // Wait for 1/2 seconds
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
            }
            return result;
        }

    }
}
