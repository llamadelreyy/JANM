using MySqlConnector;
using System.Data;
using System.Net;
using System.Net.Mail;
using PBTPro.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBTPro.DAL.Models.CommonServices;

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
        public IConfiguration _configuration { get; }
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<EmailService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EmailService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateEmailConfig();
        }

        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        public async void CreateEmailConfig()
        {
            List<EmailProp> arrItem = new List<EmailProp>();

            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
                string jsonString = await _cf.List(request);

                //Open this when the API is completed
                _Email = JsonConvert.DeserializeObject<List<EmailProp>>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar konfigurasi emel.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }
        }

        public Task<List<EmailProp>> GetEmailAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Email);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<EmailProp>> InsertEmail([FromBody] string emails = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/InsertEmail");
                string jsonString = await _cf.AddNew(request, emails, platformApiUrl + "/api/EmailConfig/InsertEmail");
                EmailProp dtEmail = JsonConvert.DeserializeObject<EmailProp>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah email config baru.", Convert.ToInt32(uID), LoggerName, "");
                return dtEmail;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteEmail(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/DeleteEmail/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk emel.", Convert.ToInt32(uID), LoggerName, "");

                return id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult<EmailProp>> UpdateEmail(int id, EmailProp dtEmail)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/EmailConfig/UpdateEmail/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtEmail), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk emel.", Convert.ToInt32(uID), LoggerName, "");

                return dtEmail;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<EmailProp>> RefreshEmailAsync()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/EmailConfig/ListEmailConfig");
                string jsonString = await _cf.List(request);
                List<EmailProp> arrItem = JsonConvert.DeserializeObject<List<EmailProp>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai konfigurasi emel.", Convert.ToInt32(uID), LoggerName, "");

                return arrItem;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }


        public async Task<string> TestEmailConfig(EmailProp entity)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

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
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return "MSG=" + ex.Message + "; SOURCE=" + ex.Source + "; STACK TRACE=" + ex.StackTrace + "; EXCEPTION=" + ex.InnerException;
            }
        }

        public async Task<bool> SendResetPwdEmail(string strUserEmail)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return false;
            }
            finally
            {
            }
        }

    }
}
