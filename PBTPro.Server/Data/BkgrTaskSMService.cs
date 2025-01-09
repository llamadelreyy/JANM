/*
Project: PBT Pro
Description: Shared service to handle background service infos
Author: Ismail
Date: December 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
07/01/2025 - initial create
*/
using Newtonsoft.Json;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;

namespace PBTPro.Data
{
    public class BkgrTaskSMService : IDisposable
    {
        public IConfiguration _configuration { get; }
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        private string _baseReqURL = "/api/BackgroundTask";
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

        public BkgrTaskSMService(IConfiguration configuration, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
        }

        public async Task<List<Tuple<string, bool>>> ListAll()
        {
            var result = new List<Tuple<string, bool>>();
            try
            {
                string requestUrl = $"{_baseReqURL}/getAllBkgrService";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<Tuple<string, bool>>>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<Tuple<string, bool>>();
            }

            return result;
        }

        public async Task<int?> getQueueBkgrService(string serviceName)
        {
            int? result = null;
            try
            {
                string requestquery = $"/{serviceName}";
                string requestUrl = $"{_baseReqURL}/getQueueBkgrService{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<int?>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public async Task<ReturnViewModel> StopBackgroundService(string serviceName)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/stopBkgrService/{serviceName}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
            }

            return result;
        }

    }
}
