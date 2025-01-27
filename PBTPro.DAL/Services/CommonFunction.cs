using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
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

        private string _baseReqURL = "/api/Audit";
        public IConfiguration _configuration { get; }
        public CommonFunction(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = contextAccessor;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl:Local");
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
            var platformApiUrl = _apiBaseUrl; //_configuration["PlatformAPI"];
           
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
            var platformApiUrl = _apiBaseUrl; //_configuration["PlatformAPI"];
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
        public async Task<string> ListJSON(HttpRequestMessage? RequestURL)
        {
            var client = new HttpClient();
            var accessToken = CheckToken();

            RequestURL.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CheckToken());
            HttpResponseMessage response = await client.SendAsync(RequestURL);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                
                var jsonObject = JObject.Parse(apiString);

                string returnMessage = jsonObject["data"]?.ToString() ?? "";
                return returnMessage;
                //var jsonArray = JArray.Parse(returnMessage);
                //if (jsonArray.Count > 0)
                //{
                //    // Convert the first element to a JSON string (no array brackets)
                //    string trimmedJsonString = jsonArray[0].ToString();
                //    return trimmedJsonString;
                //}                
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

        #region Check Token
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
        #endregion

        #region audit
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ReturnViewModel> CreateAuditLog(int intType, string strMethod, string strMessage, int userId, string uname, string moduleName, int roleid=0)
        //{
        //    try
        //    {
        //        var result = new ReturnViewModel();
        //        auditlog_info auditlog = new auditlog_info();

        //        auditlog.role_id = roleid;
        //        auditlog.module_name = string.IsNullOrEmpty(moduleName) ? "NA" : moduleName;
        //        auditlog.log_descr = strMessage;
        //        auditlog.creator_id = userId;
        //        auditlog.log_type = intType;
        //        auditlog.username = uname;
        //        auditlog.log_method = strMethod;

        //        var accessToken = CheckToken();
        //        var platformApiUrl = _apiBaseUrl;//_configuration["PlatformAPI"];
        //        var request = CheckRequest("/api/Audit/InsertAudit");

        //        var client = new HttpClient();
        //        client.BaseAddress = new Uri(platformApiUrl);
        //        string value = JsonConvert.SerializeObject(auditlog);
        //        StringContent jsonContent = new(value, Encoding.UTF8, "application/json");
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //        HttpResponseMessage response = await client.PostAsync("/api/Audit/InsertAudit", jsonContent).ConfigureAwait(false);

        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var results = await response.Content.ReadAsStringAsync();
        //            return result;
        //        }
        //        else
        //        {
        //            //================================== Comment For Testing - Azmee =====================================
        //            //////var errorContent = await response.Content.ReadAsStringAsync();
        //            //////throw new Exception($"API request failed with status code {response.StatusCode}: {errorContent}");
        //            return result;
        //            //====================================================================================================
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }           
        //    //skip for development - azmee
        //    //await Task.Delay(500); // Wait for 1/2 seconds
        //    //return true;
        //}

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
