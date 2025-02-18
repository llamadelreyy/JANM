using PBTPro.DAL.Services;
using PBTPro.DAL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models;
using System.Reflection;

namespace PBTPro.Data
{
    public class DashboardService : IDisposable
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
        private readonly ILogger<DashboardService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        private string _baseReqURL = "/api/Dashboard";
        private string LoggerName = "";

        public DashboardService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<DashboardService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        }

        [HttpGet]
        public async Task<mst_dashboard> ListAll()
        {
            var result = new mst_dashboard();

            try
            {
                //JumlahNotisSP
                string requestUrl = $"{_baseReqURL}/JumlahNotisSP";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahNotisSP = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahNotisSP = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahPremisBerlesenSP
                requestUrl = $"{_baseReqURL}/JumlahPremisBerlesenSP";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahPremisBerlesenSP = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahPremisBerlesenSP = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahNotis
                requestUrl = $"{_baseReqURL}/JumlahNotis";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahNotis = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahNotis = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahKompaun
                requestUrl = $"{_baseReqURL}/JumlahKompaun";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahKompaun = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahKompaun = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahNotaPemeriksaan
                requestUrl = $"{_baseReqURL}/JumlahNotaPemeriksaan";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahNotaPemeriksaan = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahNotaPemeriksaan = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahSitaan
                requestUrl = $"{_baseReqURL}/JumlahSitaan";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahSitaan = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahSitaan = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahPremisBerlesen
                requestUrl = $"{_baseReqURL}/JumlahPremisBerlesen";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahPremisBerlesen = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahPremisBerlesen = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahPremisTidakBerlesen
                requestUrl = $"{_baseReqURL}/JumlahPremisTidakBerlesen";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahPremisTidakBerlesen = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahPremisTidakBerlesen = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahLesenAktif
                requestUrl = $"{_baseReqURL}/JumlahLesenAktif";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahLesenAktif = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahLesenAktif = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahLesenTamatTempoh
                requestUrl = $"{_baseReqURL}/JumlahLesenTamatTempoh";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahLesenTamatTempoh = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahLesenTamatTempoh = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahLesenGantung
                requestUrl = $"{_baseReqURL}/JumlahLesenGantung";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahLesenGantung = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahLesenGantung = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //JumlahLesenTiadaData
                requestUrl = $"{_baseReqURL}/JumlahLesenTiadaData";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.JumlahLesenTiadaData = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.JumlahLesenTiadaData = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //PertambahanLesenTahunSemasa
                requestUrl = $"{_baseReqURL}/PertambahanLesenTahunSemasa";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.PertambahanLesenTahunSemasa = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.PertambahanLesenTahunSemasa = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

                //PertambahanLesenBulanSemasa
                requestUrl = $"{_baseReqURL}/PertambahanLesenBulanSemasa";
                response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result.PertambahanLesenBulanSemasa = JsonConvert.DeserializeObject<int>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai cara serahan.", 1, LoggerName, "");
                }
                else
                {
                    result.PertambahanLesenBulanSemasa = 0;
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new mst_dashboard();
            }
            return result;
        }


    }
}
