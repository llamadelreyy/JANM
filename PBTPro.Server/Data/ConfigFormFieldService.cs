/*
Project: PBT Pro
Description: Shared service to handle Param Form Field
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
13/11/2024 - initial create
*/
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Text;

namespace PBTPro.Data
{
    public partial class ConfigFormFieldService : IDisposable
    {
        public IConfiguration _configuration { get; }
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        private string _baseReqURL = "/api/ConfigFormField";
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

        public List<dynamic> fieldType = new List<dynamic>
        {
            new { Value = "hidden", Text = "Hidden Box" },
            new { Value = "text", Text = "Text Box" },
            new { Value = "datetime", Text = "Date Time" },
            new { Value = "file", Text = "File Upload" },
            new { Value = "textarea", Text = "Text Area" },
            new { Value = "dropdown", Text = "Drop Down" },
            new { Value = "radio", Text = "Radio Button" },
            new { Value = "checkbox", Text = "Check Box" }
        };

        public ConfigFormFieldService(IConfiguration configuration, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
        }

        public async Task<List<config_form_field_view>> ListAll()
        {
            var result = new List<config_form_field_view>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetList";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<config_form_field_view>>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<config_form_field_view>();
            }

            return result;
        }

        public async Task<List<config_form_field_view>> ListByFormType(string formType)
        {
            var result = new List<config_form_field_view>();
            try
            {
                string requestquery = $"?formType={formType}";
                string requestUrl = $"{_baseReqURL}/GetListByFormType{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<config_form_field_view>>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<config_form_field_view>();
            }

            return result;
        }

        public async Task<config_form_field_view> ViewDetail(int id)
        {
            var result = new config_form_field_view();
            try
            {
                string requestquery = $"/{id}";
                string requestUrl = $"{_baseReqURL}/GetDetail{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<config_form_field_view>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new config_form_field_view();
            }

            return result;
        }
        
        public async Task<ReturnViewModel> Add(config_form_field_view inputModel)
        {

            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
            }

            return result;
        }

        public async Task<ReturnViewModel> Update(config_form_field_view inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel.field_id;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
            }

            return result;
        }

        public async Task<ReturnViewModel> Delete(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/Remove/{id}";
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
