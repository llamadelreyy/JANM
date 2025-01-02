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

        public IConfiguration _configuration { get; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<EmailService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/Unit";
        private string LoggerName = "";
        private List<email_config> _Email { get; set; }

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EmailService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        }
        [HttpGet]
        public async Task<List<email_config>> ListAll()
        {
            var result = new List<email_config>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<email_config>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<email_config>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<email_config>> Refresh()
        {
            var result = new List<email_config>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<email_config>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<email_config>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(email_config inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, email_config inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.email_id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk jadual rondaan.", 1, LoggerName, "");

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
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
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public Task<List<email_config>> GetUnitAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");
            return Task.FromResult(_Email);
        }

        public async Task<email_config> ViewDetail(int id)
        {
            var result = new email_config();
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
                        result = JsonConvert.DeserializeObject<email_config>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new email_config();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
    }
}

        //private List<email_config> _Email { get; set; }
        //public IConfiguration _configuration { get; }
        //private readonly PBTProDbContext _dbContext;
        //private readonly ILogger<EmailService> _logger;
        //public IConfiguration _configuration { get; }
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //protected readonly AuditLogger _cf;
        //private readonly ApiConnector _apiConnector;
        //private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        //private string _baseReqURL = "/api/EmailConfig";
        //private string LoggerName = "";

        //public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EmailService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        //{
        //    _configuration = configuration;
        //    _httpContextAccessor = httpContextAccessor;
        //    _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        //    _logger = logger;
        //    _dbContext = dbContext;
        //    _PBTAuthStateProvider = PBTAuthStateProvider;
        //    _apiConnector = apiConnector;
        //    _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
        //    CreateEmailConfig();
        //}

        //public Task<List<EmailProp>> GetEmailAsync(CancellationToken ct = default)
        //{
        //    return Task.FromResult(_Email);
        //}
    
        //public async void CreateEmailConfig()
        //{
        //    List<email_config> arrItem = new List<email_config>();

        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
        //        string jsonString = await _cf.List(request);

        //        //Open this when the API is completed
        //        _Email = JsonConvert.DeserializeObject<List<email_config>>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar konfigurasi emel.", Convert.ToInt32(uID), LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //    }
        //}

        //public Task<List<email_config>> GetEmailAsync(CancellationToken ct = default)
        //{
        //    return Task.FromResult(_Email);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult<email_config>> InsertEmail([FromBody] string emails = "")
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/InsertEmail");
        //        string jsonString = await _cf.AddNew(request, emails, platformApiUrl + "/api/EmailConfig/InsertEmail");
        //        email_config dtEmail = JsonConvert.DeserializeObject<email_config>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah email config baru.", Convert.ToInt32(uID), LoggerName, "");
        //        return dtEmail;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        _Email = new List<EmailProp>();
        //    }
        //}

        //[HttpGet]
        //public async Task<List<EmailProp>> RefreshEmailAsync()
        //{
        //    var result = new List<EmailProp>();
        //    try
        //    {
        //        string requestUrl = $"{_baseReqURL}/ListEmailConfig";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl);

        //        if (response.ReturnCode == 200)
        //        {
        //            string? dataString = response?.Data?.ToString();
        //            if (!string.IsNullOrWhiteSpace(dataString))
        //            {
        //                result = JsonConvert.DeserializeObject<List<EmailProp>>(dataString);
        //                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
        //            }
        //            else
        //            {
        //                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
        //            }
        //        }

        //[AllowAnonymous]
        //[HttpPut]
        //public async Task<ActionResult<email_config>> UpdateEmail(int id, email_config dtEmail)
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var uri = platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id;
        //        var request = _cf.CheckRequestPut(platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id);
        //        string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtEmail), uri);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk emel.", Convert.ToInt32(uID), LoggerName, "");

        //        return dtEmail;

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
        //public async Task<List<email_config>> RefreshEmailAsync()
        //{
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
        //        string jsonString = await _cf.List(request);
        //        List<email_config> arrItem = JsonConvert.DeserializeObject<List<email_config>>(jsonString);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai konfigurasi emel.", Convert.ToInt32(uID), LoggerName, "");

        //        return arrItem;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return false;
        //    }
        //    finally
        //    {
        //    }
        //}


        //public async Task<string> TestEmailConfig(email_config entity)
        //{
        //    try
        //    {
        //        using (var message = new MailMessage())
        //        {
        //            message.To.Add(new MailAddress(entity.smtp_email, "Administrator"));
        //            message.From = new MailAddress(entity.smtp_email, entity.smtp_sender);
        //            message.Subject = "Email Testing";
        //            message.Body = "Hooray! Email setting is working.";
        //            message.IsBodyHtml = false;

        //            using (var client = new SmtpClient(entity.smtp_host))
        //            {
        //                client.UseDefaultCredentials = false;
        //                client.Port = int.Parse(entity.smtp_port);
        //                client.Credentials = new NetworkCredential(entity.smtp_user, entity.smtp_password);
        //                client.EnableSsl = entity.smtp_protocol;
        //                await client.SendMailAsync(message);
        //            }

        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //        return "MSG=" + ex.Message + "; SOURCE=" + ex.Source + "; STACK TRACE=" + ex.StackTrace + "; EXCEPTION=" + ex.InnerException;
        //    }
        //}

        //public async Task<ReturnViewModel> DeleteEmail(int id)
        //{
        //    var result = new ReturnViewModel();
        //    try
        //    {
        //        string requestUrl = $"{_baseReqURL}/DeleteEmail/{id}";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

        //        result = response;
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk emel.", 1, LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new ReturnViewModel();
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //    return result;
        //}

        //public async Task<ReturnViewModel> InsertEmail(faq_info inputModel)
        //{
        //    var result = new ReturnViewModel();
        //    try
        //    {
        //        var reqData = JsonConvert.SerializeObject(inputModel);
        //        var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

        //        string requestUrl = $"{_baseReqURL}/InsertEmail";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

        //        result = response;
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah email config baru.", 1, LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new ReturnViewModel();
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //    return result;
        //}

        //public async Task<ReturnViewModel> UpdateEmail(int id, EmailProp inputModel)
        //{
        //    var result = new ReturnViewModel();
        //    try
        //    {
        //        var reqData = JsonConvert.SerializeObject(inputModel);
        //        var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

        //        string requestUrl = $"{_baseReqURL}/UpdateEmail/{id}";
        //        var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

        //        result = response;
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk emel.", 1, LoggerName, "");

        //    }
        //    catch (Exception ex)
        //    {
        //        result = new ReturnViewModel();
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //    return result;
        //}

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

//    }
//}
