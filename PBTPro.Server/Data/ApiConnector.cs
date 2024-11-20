/*
Project: PBT Pro
Description: Shared service to handle API request & response
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
07/11/2024 - initial create
*/
using Microsoft.AspNetCore.Mvc;
using PBTPro.DAL.Models.CommonServices;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PBTPro.Data
{
    public class ApiConnector
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public string accessToken { get; set; } = "";

        public ApiConnector(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string?> getBaseApiURL(string? type = "Local")
        {
            string result = "https://localhost:7020";
            try
            {
                if (type.ToUpper() != "LOCAL")
                {
                    result = _config["ApiBaseUrl:Public"];
                }
                else
                {
                    result = _config["ApiBaseUrl:Local"];
                }
            }
            catch (Exception e)
            {
                result = "https://localhost:7020";
            }

            return result;
        }

        public async Task<ReturnViewModel> ProcessLocalApi(string requestUrl, [FromQuery] HttpMethod? RequestMethod = null, [FromBody] HttpContent? RequestContent = null)
        {
            var result = new ReturnViewModel();
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                requestUrl = _config["ApiBaseUrl:Local"] + requestUrl;

                if (RequestMethod == null)
                {
                    RequestMethod = HttpMethod.Get;
                }

                var request = new HttpRequestMessage(RequestMethod, requestUrl);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                if (RequestContent != null)
                {
                    request.Content = RequestContent;
                }

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseBody))
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;

                        result.ReturnCode = root.TryGetProperty("returnCode", out JsonElement returnCodeProp)
                            ? returnCodeProp.GetInt32()
                            : 200;

                        result.ReturnMessage = root.TryGetProperty("returnMessage", out JsonElement returnMessageProp)
                            ? returnMessageProp.GetString()
                            : "";

                        if (root.TryGetProperty("returnParameter", out JsonElement returnParameterProp) && returnParameterProp.ValueKind == JsonValueKind.Array)
                        {
                            result.ReturnParameter = new List<string>();
                            foreach (JsonElement item in returnParameterProp.EnumerateArray())
                            {
                                result.ReturnParameter.Add(item.GetString());
                            }
                        }
                        else
                        {
                            result.ReturnParameter = null;
                        }

                        result.Status = root.TryGetProperty("status", out JsonElement statusProp)
                            ? statusProp.GetString()
                            : "OK";

                        result.DateTime = root.TryGetProperty("dateTime", out JsonElement dateTimeProp)
                            ? DateTime.Parse(dateTimeProp.GetString())
                        : DateTime.Now;

                        if (root.TryGetProperty("data", out JsonElement dataProp))
                        {
                            // If "data" is null, assign null to result.Data
                            if (dataProp.ValueKind == JsonValueKind.Null)
                            {
                                result.Data = null;
                            }
                            else
                            {
                                // Convert the value of "data" to string
                                result.Data = dataProp.ToString();
                            }
                        }
                        else
                        {
                            result.Data = null; // "data" does not exist
                        }
                    }

                    /* This is an original approach, commented out to try using JsonDocument for performance testing. 
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                    result.ReturnCode = Convert.ToInt32(jsonResponse["returnCode"] ?? 200);
                    result.ReturnMessage = jsonResponse["returnMessage"]?.ToString() ?? "";
                    result.ReturnParameter = jsonResponse.ContainsKey("returnParameter")
                        ? JsonConvert.DeserializeObject<List<string>>(jsonResponse["returnParameter"].ToString())
                        : null;
                    result.Status = jsonResponse["status"]?.ToString() ?? "OK";
                    result.DateTime = DateTime.Parse(jsonResponse["dateTime"]?.ToString() ?? DateTime.Now.ToString());
                    result.Data = jsonResponse["data"];
                    */
                }
                else
                {
                    result.ReturnMessage = response.ReasonPhrase ?? "";
                    result.ReturnCode = (int)response.StatusCode;

                    if (response.Headers.Contains("return-message"))
                    {
                        result.ReturnMessage = response.Headers.GetValues("return-message").ToString();
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                result.ReturnCode = 500;
                result.ReturnMessage = "Invalid JSON response: " + jsonEx.Message;
                result.Status = "Error";
                result.DateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                result.ReturnCode = 500;
                result.ReturnMessage = e.Message;
                result.Status = "Error";
                result.DateTime = DateTime.Now;
            }

            return result;
        }

        public async Task<ReturnViewModel> ProcessPublicApi(string requestUrl, [FromQuery] HttpMethod? RequestMethod = null, [FromBody] HttpContent? RequestContent = null, string? accessToken = null)
        {
            var result = new ReturnViewModel();
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                requestUrl = _config["ApiBaseUrl:Public"] + requestUrl;

                if (RequestMethod == null)
                {
                    RequestMethod = HttpMethod.Get;
                }

                var request = new HttpRequestMessage(RequestMethod, requestUrl);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                if (RequestContent != null)
                {
                    request.Content = RequestContent;
                }

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseBody))
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;

                        result.ReturnCode = root.TryGetProperty("returnCode", out JsonElement returnCodeProp)
                            ? returnCodeProp.GetInt32()
                            : 200;

                        result.ReturnMessage = root.TryGetProperty("returnMessage", out JsonElement returnMessageProp)
                            ? returnMessageProp.GetString()
                            : "";

                        if (root.TryGetProperty("returnParameter", out JsonElement returnParameterProp) && returnParameterProp.ValueKind == JsonValueKind.Array)
                        {
                            result.ReturnParameter = new List<string>();
                            foreach (JsonElement item in returnParameterProp.EnumerateArray())
                            {
                                result.ReturnParameter.Add(item.GetString());
                            }
                        }
                        else
                        {
                            result.ReturnParameter = null;
                        }

                        result.Status = root.TryGetProperty("status", out JsonElement statusProp)
                            ? statusProp.GetString()
                            : "OK";

                        result.DateTime = root.TryGetProperty("dateTime", out JsonElement dateTimeProp)
                            ? DateTime.Parse(dateTimeProp.GetString())
                        : DateTime.Now;

                        if (root.TryGetProperty("data", out JsonElement dataProp))
                        {
                            // If "data" is null, assign null to result.Data
                            if (dataProp.ValueKind == JsonValueKind.Null)
                            {
                                result.Data = null;
                            }
                            else
                            {
                                // Convert the value of "data" to string
                                result.Data = dataProp.ToString();
                            }
                        }
                        else
                        {
                            result.Data = null; // "data" does not exist
                        }
                    }

                    /* This is an original approach, commented out to try using JsonDocument for performance testing. 
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                    result.ReturnCode = Convert.ToInt32(jsonResponse["returnCode"] ?? 200);
                    result.ReturnMessage = jsonResponse["returnMessage"]?.ToString() ?? "";
                    result.ReturnParameter = jsonResponse.ContainsKey("returnParameter")
                        ? JsonConvert.DeserializeObject<List<string>>(jsonResponse["returnParameter"].ToString())
                        : null;
                    result.Status = jsonResponse["status"]?.ToString() ?? "OK";
                    result.DateTime = DateTime.Parse(jsonResponse["dateTime"]?.ToString() ?? DateTime.Now.ToString());
                    result.Data = jsonResponse["data"];
                    */
                }
                else
                {
                    result.ReturnMessage = response.ReasonPhrase ?? "";
                    result.ReturnCode = (int)response.StatusCode;

                    if (response.Headers.Contains("return-message"))
                    {
                        result.ReturnMessage = response.Headers.GetValues("return-message").ToString();
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                result.ReturnCode = 500;
                result.ReturnMessage = "Invalid JSON response: " + jsonEx.Message;
                result.Status = "Error";
                result.DateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                result.ReturnCode = 500;
                result.ReturnMessage = e.Message;
                result.Status = "Error";
                result.DateTime = DateTime.Now;
            }

            return result;
        }
    }
}
