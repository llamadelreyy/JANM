using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace PBTPro.Data
{
    public class EmailService : IDisposable
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
        private List<EmailProp> _Email { get; set; }
        private readonly PBTProDbContext _dbContext;
        private readonly ILogger<EmailService> _logger;
        public IConfiguration _configuration { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/EmailConfig";
        private string LoggerName = "";

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EmailService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cf = new AuditLogger(configuration, apiConnector);
            _logger = logger;
            _dbContext = dbContext;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            CreateEmailConfig();
        }

        public Task<List<EmailProp>> GetEmailAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Email);
        }
    
        public async void CreateEmailConfig()
        {
            string requestUrl = $"{_baseReqURL}/ListEmailConfig";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        _Email = JsonConvert.DeserializeObject<List<EmailProp>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar konfigurasi emel.", 1, LoggerName, "");
                    }
                    else
                    {
                        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                    }
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                _Email = new List<EmailProp>();
            }
        }

        [HttpGet]
        public async Task<List<EmailProp>> RefreshEmailAsync()
        {
            var result = new List<EmailProp>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListEmailConfig";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<EmailProp>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                    }
                    else
                    {
                        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                    }
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<EmailProp>();
            }
            return result;
        }

        public async Task<bool> SendResetPwdEmail(string strUserEmail)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                return false;
            }
            finally
            {
            }
        }

        public async Task<string> TestEmailConfig(EmailProp entity)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(new MailAddress(entity.smtpEmail, "Administrator"));
                    message.From = new MailAddress(entity.smtpEmail, entity.smtpSender);
                    message.Subject = "Email Testing";
                    message.Body = "Hooray! Email setting is working.";
                    message.IsBodyHtml = false;

                    using (var client = new SmtpClient(entity.smtpHost))
                    {
                        client.UseDefaultCredentials = false;
                        client.Port = int.Parse(entity.smtpPort);
                        client.Credentials = new NetworkCredential(entity.smtpUser, entity.smtpPassword);
                        client.EnableSsl = entity.smtpProtocol;
                        await client.SendMailAsync(message);
                    }

                    return "";
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                return "MSG=" + ex.Message + "; SOURCE=" + ex.Source + "; STACK TRACE=" + ex.StackTrace + "; EXCEPTION=" + ex.InnerException;
            }
        }

        public async Task<ReturnViewModel> DeleteEmail(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/DeleteEmail/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk emel.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> InsertEmail(faq_info inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/InsertEmail";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah email config baru.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> UpdateEmail(int id, EmailProp inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/UpdateEmail/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk emel.", 1, LoggerName, "");

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        //public async void CreateEmailConfig()
        //{
        //    List<EmailProp> arrItem = new List<EmailProp>();
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
        //        string jsonString = await _cf.List(request);

        //        //Open this when the API is completed
        //        _Email = JsonConvert.DeserializeObject<List<EmailProp>>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar konfigurasi emel.", 1, LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //}

        //[HttpGet]
        //public async Task<List<EmailProp>> CreateEmailConfigs()
        //{
        //    var result = new List<EmailProp>();
        //    string requestUrl = $"{_baseReqURL}/ListEmailConfig";
        //    var response = await _apiConnector.ProcessLocalApi(requestUrl);

        //    try
        //    {

        //        if (response.ReturnCode == 200)
        //        {
        //            string? dataString = response?.Data?.ToString();
        //            if (!string.IsNullOrWhiteSpace(dataString))
        //            {
        //                result = JsonConvert.DeserializeObject<List<EmailProp>>(dataString);
        //                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar konfigurasi emel.", 1, LoggerName, "");
        //            }
        //            else
        //            {
        //                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        result = new List<EmailProp>();
        //    }
        //    return result;
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<List<EmailProp>> RefreshEmailAsync()
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
        //        string jsonString = await _cf.List(request);
        //        List<EmailProp> arrItem = JsonConvert.DeserializeObject<List<EmailProp>>(jsonString);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai konfigurasi emel.", 1, LoggerName, "");

        //        return arrItem;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult<EmailProp>> InsertEmail([FromBody] string emails = "")
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/InsertEmail");
        //        string jsonString = await _cf.AddNew(request, emails, platformApiUrl + "/api/EmailConfig/InsertEmail");
        //        EmailProp dtEmail = JsonConvert.DeserializeObject<EmailProp>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah email config baru.", 1, LoggerName, "");
        //        return dtEmail;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}

        //[HttpDelete]
        //public async Task<int> DeleteEmail(int id)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/DeleteEmail/" + id);
        //        string jsonString = await _cf.Delete(request);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk emel.", 1, LoggerName, "");

        //        return id;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return 0;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPut]
        //public async Task<ActionResult<EmailProp>> UpdateEmail(int id, EmailProp dtEmail)
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var uri = platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id;
        //        var request = _cf.CheckRequestPut(platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id);
        //        string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtEmail), uri);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk emel.", 1, LoggerName, "");

        //        return dtEmail;

        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return null;
        //    }
        //}       

    }
}
