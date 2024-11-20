using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PBTPro.Api.Constants;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Net.Http.Headers;

namespace PBTPro.Api.Controllers.Base
{
    [Authorize]
    public class IBaseController : ControllerBase
    {
        protected readonly PBTProDbContext _dbContext;
        protected readonly string? _apiBaseUrl;

        public IBaseController(PBTProDbContext dbContext)
        {
            _dbContext = dbContext;
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
        protected OkObjectResult Ok<T>(T data, string returnMessage = "", List<string>? returnParameter = null)
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

        protected PaginationInfo GetPaginationInfo(int count, int? skip, int? take)
        {
            var pageInfo = new PaginationInfo();
            decimal tp = (decimal)count / (decimal)take!.Value;
            int totalPage = (int)Math.Ceiling(tp);
            int page = (skip!.Value / take!.Value) + 1;

            pageInfo.TotalPages = totalPage;
            pageInfo.TotalRecords = count;
            pageInfo.RecordPerPage = take!.Value;
            pageInfo.CurrentPageNo = page;

            return pageInfo;
        }

        protected OkObjectResult Ok<T>(T data, PaginationInfo paginationInfo, string returnMessage = "", List<string>? returnParameter = null)
        {
            return base.Ok(new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.Success,
                Status = "OK",
                Data = data,
                PaginationInfo = paginationInfo,
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
            string? result = "system";
            try
            {
                result = User?.Identity?.Name;
            }
            catch (Exception ex)
            {
                result = "system";
            }
            return result ?? "system";
        }

        protected async Task<int> getDefRunUserId()
        {
            return 0;
        }

        protected string SystemMesg(string features, string code, MessageTypeEnum type, string msg, List<string>? param = null)
        {
            var name = ".";
            code = "." + code.ToUpper().Replace(" ", "_");

            config_system_message model = new config_system_message();
            model.message_feature = features.ToUpper();
            switch (type) //S-Success, W-Warning, E-Error
            {
                case MessageTypeEnum.Success:
                    model.message_type = "S";
                    break;
                case MessageTypeEnum.Warning:
                    model.message_type = "W";
                    break;
                case MessageTypeEnum.Error:
                    model.message_type = "E";
                    break;
                case MessageTypeEnum.Alert:
                    model.message_type = "A";
                    break;
                default:
                    model.message_type = "S";
                    break;
            }
            model.message_code = model.message_feature + name + model.message_type + code;
            model.message_body = msg;

            var retMsg = GetSysMesg(model.message_code, model);
            if (retMsg != null)
            {
                msg = retMsg.message_body;
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

        private config_system_message? GetSysMesg(string code, config_system_message? model)
        {
            config_system_message? appSystemMessage = null;
            try
            {
                appSystemMessage = _dbContext.config_system_messages.Where(m => m.message_code == code).FirstOrDefault();
                if (appSystemMessage == null && model != null)
                {
                    appSystemMessage = new config_system_message
                    {
                        message_feature = model.message_feature,
                        message_code = model.message_code,
                        message_type = model.message_type,
                        message_body = model.message_body,
                        //created_by = User?.Identity?.Name! ?? "anonymous",
                        created_date = DateTime.Now
                    };
                    _dbContext.config_system_messages.Add(appSystemMessage);
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
                    try
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
                    }
                    catch (Exception ex)
                    {
                        throw;
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
