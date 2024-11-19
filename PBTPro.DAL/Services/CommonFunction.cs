using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBTPro.DAL.Models;
using Serilog;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PBTPro.DAL.Services
{
    public class CommonFunction : ControllerBase
    {
        Configuration configuration = null;

        protected readonly string? _dbConn;
        protected readonly string? _apiBaseUrl;
        private readonly ILogger? _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        public IConfiguration _configuration { get; }
        public CommonFunction(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = contextAccessor;
            //_session = contextAccessor.HttpContext.Session;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
        }
        public void LogConsole(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            // final output
            Console.WriteLine("(" + caller + ") " + message + " PASS at line " + lineNumber);
        }
        public void LogMessage(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            Log.Warning("(" + caller + ") " + message + " PASS at line " + lineNumber);
        }
        public void LogError(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            Log.Error("(" + caller + ") " + message + " at line " + lineNumber);
        }

        #region COMMON CRUD FUNCTION TO CALL API
        [HttpGet]
        public async Task<string> Retrieve(HttpRequestMessage? RequestURL, string? accessToken)
        {
            var client = new HttpClient();

            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.SendAsync(RequestURL);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                //var value = JObject.Parse(apiString);
                //string strValue = value.ToString();
                return apiString;
            }
            return "";
        }
        [HttpPost]
        public async Task<string> AddNew(HttpRequestMessage? RequestURL, string JsonStr, string URIRequest)
        {
            var client = new HttpClient();
            var accessToken = CheckToken();
            var platformApiUrl = _configuration["PlatformAPI"];
           
            client.BaseAddress = new Uri(platformApiUrl);

            StringContent jsonContent = new(JsonStr, Encoding.UTF8, "application/json");
            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.PostAsync(URIRequest, jsonContent).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var result = await response.Content.ReadAsStringAsync();
                //var value = JObject.Parse(result);
                //string strValue = value.ToString();
                return result;
            }
            return "";
        }
        [HttpPut]
        public async Task<string> Update(HttpRequestMessage? c, string JsonStr, string URIRequest)
        {
            var client = new HttpClient();
            var accessToken = CheckToken(); 
            var platformApiUrl = _configuration["PlatformAPI"];
            client.BaseAddress = new Uri(platformApiUrl);

            // Create the request message
            using var requestMessage = new HttpRequestMessage(HttpMethod.Put, URIRequest)
            {
                Content = new StringContent(JsonStr, Encoding.UTF8, "application/json")
            };

            // Set the authorization header
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send the PUT request
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode(); // Throw if not a success code.

                // Read the response content
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (HttpRequestException ex)
            {
                // Log and handle errors
                return string.Empty;
            }
        }

        [HttpGet]
        public async Task<string> List(HttpRequestMessage? RequestURL)
        {
            var client = new HttpClient();
            var accessToken = CheckToken();

            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CheckToken());
            HttpResponseMessage response = await client.SendAsync(RequestURL);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                //var value = JObject.Parse(apiString);
                //string JsonStr = value.ToString();
                return apiString;
            }
            return "";
        }

        [HttpGet]
        public async Task<string> GetSingleDataString(HttpRequestMessage? RequestURL)
        {
            var client = new HttpClient();
            var accessToken = CheckToken();

            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CheckToken());
            HttpResponseMessage response = await client.SendAsync(RequestURL);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                return apiString;
            }
            return "";
        }
        [HttpDelete]
        public async Task<string> Delete(HttpRequestMessage? RequestURL)
        {
            var client = new HttpClient();
            var accessToken = CheckToken();

            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.DeleteAsync(RequestURL.RequestUri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return "";
        }
        #endregion

        public string ReadConfig(string strKey)
        {
            string absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            absolutePath = absolutePath.Replace("%20", " ");
            try
            {
                configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(absolutePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (configuration != null)
            {
                return GetAppSetting(configuration, strKey);
            }

            return string.Empty;
        }
        public static string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement keyValueConfigurationElement = config.AppSettings.Settings[key];
            if (keyValueConfigurationElement != null)
            {
                string value = keyValueConfigurationElement.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }
        [AllowAnonymous]
        public string CheckToken()
        {
            var accessToken = _session?.GetString("Token");
            return accessToken;
        }

        public HttpRequestMessage CheckRequest(string RequestURL)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}" + RequestURL);
            return request;
        }

        public HttpRequestMessage CheckRequestPut(string RequestURL)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_apiBaseUrl}" + RequestURL);
            return request;
        }

        #region audit

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CreateAuditLog(int intType, string strMethod, string strMessage, int userId, string uname, string moduleName, int roleid=0)
        {
            //////try
            //////{
            //////    AuditlogInfo auditlog = new AuditlogInfo();

            //////    auditlog.AuditRoleId = roleid;
            //////    auditlog.AuditModuleName = string.IsNullOrEmpty(moduleName) ? "NA" : moduleName;
            //////    auditlog.AuditDescription = strMessage;
            //////    auditlog.CreatedBy = userId;
            //////    auditlog.AuditType = intType;                
            //////    auditlog.AuditUsername = uname;                
            //////    auditlog.AuditMethod = strMethod;

            //////    //Calling api to perform addnew audit transaction
            //////    var accessToken = CheckToken();
            //////    var platformApiUrl = _configuration["PlatformAPI"];
            //////    var request = CheckRequest("/api/Audit/InsertAudit");

            //////    var client = new HttpClient();
            //////    client.BaseAddress = new Uri(platformApiUrl);
            //////    string value = JsonConvert.SerializeObject(auditlog);
            //////    StringContent jsonContent = new(value, Encoding.UTF8, "application/json");
            //////    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //////    HttpResponseMessage response = await client.PostAsync("/api/Audit/InsertAudit", jsonContent).ConfigureAwait(false);

            //////    if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //////    {
            //////        var results= await response.Content.ReadAsStringAsync();
            //////        return true;
            //////    }
            //////    else
            //////    {                   
            //////        var errorContent = await response.Content.ReadAsStringAsync();

            //////        throw new Exception($"API request failed with status code {response.StatusCode}: {errorContent}");
            //////    }
            //////}
            //////catch (Exception ex)
            //////{
            //////    throw (ex);
            //////}
            ///

            //skip for development - azmee
            await Task.Delay(500); // Wait for 1/2 seconds
            return true;
        }

        /// <summary>
        /// Get current method name
        /// </summary>
        /// <returns>Return method</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }


        #endregion
    }
}
