using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PBTPro.Api.Constants;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;
using System.Net.Http.Headers;

namespace PBTPro.Api.Controllers.Base
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class IBaseController : ControllerBase
    {
        protected readonly IWebHostEnvironment _environment;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string? _baseUploadDir;
        protected readonly string? _apiBaseUrl;
        protected readonly string? _MKPublicUrl;
        protected readonly PBTProDbContext _dbContext;

        public IBaseController(IWebHostEnvironment environment, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, string? baseUploadDir, string? apiBaseUrl, string? mKPublicUrl)
        {
            _environment = environment;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _baseUploadDir = baseUploadDir;
            _apiBaseUrl = apiBaseUrl;
            _MKPublicUrl = mKPublicUrl;
        }

        public IBaseController(PBTProDbContext dbContext)
        {
        }

        /// <summary>
        /// OK data - 200
        /// </summary>
        /// <typeparam name="T">object</typeparam>
        /// <param name="data">Data object return to caller</param>
        /// <param name="paginationInfo">Pagination Info</param>
        /// <param name="returnMessage">Message to caller</param>
        /// <param name="returnParameter">Parameter for returnMessage</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.OkObjectResult for the response.</returns>
        protected OkObjectResult Ok<T>(T data,  string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.Ok(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.Success,
                Status = "OK",
                Data = data,               
                ReturnMessage = returnMessage,
                ReturnParameter = returnParameter
            });
        }
        /// <summary>
        /// Failed - Internal Server Error - 500
        /// </summary>
        /// <typeparam name="T">object</typeparam>
        /// <param name="data">Data object return to caller</param>
        /// <param name="returnMessage">Message to caller</param>
        /// <param name="returnParameter">Parameter for returnMessage</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.OkObjectResult for the response.</returns>
        protected BadRequestObjectResult Error<T>(T data, string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.BadRequest(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.Error,
                Status = "Error",
                Data = data,
                ReturnMessage = returnMessage,
                ReturnParameter = returnParameter
            });
        }
        /// <summary>
        /// Unauthorized - 401
        /// </summary>
        /// <typeparam name="T">object</typeparam>
        /// <param name="data">Data object return to caller</param>
        /// <param name="returnMessage">Message to caller</param>
        /// <param name="returnParameter">Parameter for returnMessage</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.OkObjectResult for the response.</returns>
        protected UnauthorizedObjectResult Unauthorized<T>(T data, string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.Unauthorized(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.NoRequestAuthority,
                Status = "Unauthorized",
                Data = data,
                ReturnMessage = returnMessage,
                ReturnParameter = returnParameter
            });
        }
        /// <summary>
        /// Not Found - 404.
        /// Don't use for record not found.
        /// </summary>
        /// <typeparam name="T">object</typeparam>
        /// <param name="data">Data object return to caller</param>
        /// <param name="returnMessage">Message to caller</param>
        /// <param name="returnParameter">Parameter for returnMessage</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.NotFoundObjectResult for the response.</returns>
        protected NotFoundObjectResult NotFound<T>(T data, string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.NotFound(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.NotFound,
                Status = "NotFound",
                Data = data,
                ReturnMessage = returnMessage,
                ReturnParameter = returnParameter
            });
        }
        /// <summary>
        /// No Content / Data Not Found - 204
        /// </summary>
        /// <param name="returnMessage">Message to caller at Header</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.NoContentResult for the response.</returns>
        protected NoContentResult NoContent(string returnMessage = "")
        {
            Response.Headers.Add("return-message", returnMessage);
            Response.Headers.Add("return-server", Environment.MachineName);
            return base.NoContent();
        }
        /// <summary>
        /// Bad Request - 400
        /// </summary>
        /// <typeparam name="T">object</typeparam>
        /// <param name="data">Data object return to caller</param>
        /// <param name="returnMessage">Message to caller</param>
        /// <param name="returnParameter">Parameter for returnMessage</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.OkObjectResult for the response.</returns>
        protected BadRequestObjectResult BadRequest<T>(T data, string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.BadRequest(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.BadRequest,
                Status = "BadRequest",
                Data = data,
                ReturnMessage = returnMessage,
                ReturnParameter = returnParameter
            });
        }

        protected async Task<string> getDefRunUser()
        {
            var result = "System";
            try
            {
                result = User?.Identity?.Name;
            }
            catch (Exception ex)
            {
                result = "System";
            }
            return result;
        }

        protected string SystemMesg(string module, string code, MessageTypeEnum type, string msg, List<string>? param = null)
        {
            var name = ".";
            code = "." + code.ToUpper().Replace(" ", "_");

            AppSystemMessageModel model = new AppSystemMessageModel();
            model.Module = module.ToUpper();
            switch (type) //S-Success, W-Warning, E-Error
            {
                case MessageTypeEnum.Success:
                    model.Type = "S";
                    break;
                case MessageTypeEnum.Warning:
                    model.Type = "W";
                    break;
                case MessageTypeEnum.Error:
                    model.Type = "E";
                    break;
                case MessageTypeEnum.Alert:
                    model.Type = "A";
                    break;
                default:
                    model.Type = "S";
                    break;
            }
            model.Code = model.Module + name + model.Type + code;
            model.Message = msg;

            var retMsg = GetSysMesg(model.Code, model);
            if (retMsg != null)
            {
                msg = retMsg.Message;
            }

            if (param != null && param.Count! > 0)
            {
                int k = 0;
                foreach (string par in param)
                {
                    msg = msg.Replace($"[{k}]", par);
                    k++;
                }
            }

            return msg;
        }
        private AppSystemMessage? GetSysMesg(string code, AppSystemMessageModel? model)
        {
            AppSystemMessage? appSystemMessage = null;
            try
            {
                appSystemMessage = _dbContext.AppSystemMessages.Where(m => m.Code == code).FirstOrDefault();
                if (appSystemMessage == null && model != null)
                {
                    appSystemMessage = new AppSystemMessage
                    {
                        Module = model.Module,
                        Code = model.Code,
                        Type = model.Type,
                        Message = model.Message,
                        CreatedBy = User?.Identity?.Name! ?? "anonymous",
                        CreatedDtm = DateTime.Now
                    };
                    _dbContext.AppSystemMessages.Add(appSystemMessage);
                    _dbContext.SaveChanges(false);
                }
            }
            catch (Exception ex)
            {
                appSystemMessage = null;
            }
            return appSystemMessage;
        }

        private async Task<string?> getAccessTokenAsync()
        {
            string? accessToken = null;
            try
            {
                accessToken = await HttpContext.GetTokenAsync("access_token");
            }
            catch (Exception ex)
            {
                accessToken = null;
            }

            return accessToken;
        }
        protected async Task<ReturnViewModel> ProcessAPI(string RequestURL, [FromQuery] HttpMethod? RequestMethod = null, [FromBody] HttpContent? RequestContent = null)
        {
            ReturnViewModel result = new ReturnViewModel();
            try
            {
                #region build request
                var accessToken = await getAccessTokenAsync();
                if (RequestMethod == null)
                {
                    RequestMethod = HttpMethod.Get;
                }

                var request = new HttpRequestMessage(RequestMethod, $"{_apiBaseUrl}" + RequestURL);
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
                }

                if (RequestContent != null)
                {

                    request.Content = RequestContent;
                }
                #endregion

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    #region Response Massage
                    string? jsonString = await response.Content.ReadAsStringAsync();
                    string? returnMessage = null;
                    string? dataString = null;
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        JObject jsonRS = JObject.Parse(jsonString);
                        returnMessage = jsonRS.SelectToken("returnMessage")?.ToString();
                        dataString = jsonRS.SelectToken("data")?.ToString();
                        result.DateTime = jsonRS.SelectToken("dateTime").ToObject<DateTime>();
                        result.ReturnCode = jsonRS.SelectToken("returnCode").ToObject<int>();
                        result.Status = jsonRS.SelectToken("status")?.ToString();
                        result.Data = dataString;
                        result.ReturnMessage = returnMessage;
                        result.ReturnParameter = jsonRS.SelectToken("returnParameter")?.ToObject<List<string>>();
                    }
                    else
                    {
                        result.ReturnMessage = response.ReasonPhrase?.ToString();
                        result.ReturnCode = (int)response.StatusCode;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result.ReturnMessage = response.Headers.Contains("return-message")
                        ? response.Headers.GetValues("return-message")?.FirstOrDefault()
                        : returnMessage;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                //_cf.SystemLoggerAsync("IBaseController", "", "1", User?.Identity?.Name ?? "System", "", 0, ex.Message);
                result.DateTime = DateTime.Now;
                result.ReturnCode = (int)HttpStatus.Error;
                result.Status = "Error";
                result.ReturnMessage = ex.Message;
            }
            return result;
        }
    }
}
