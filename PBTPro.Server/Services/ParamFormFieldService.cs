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
using Newtonsoft.Json;
using PBT.Data;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;
using PBTPro.Shared.Models.RequestPayLoad;
using System.Text;

namespace PBT.Services
{
    public partial class ParamFormFieldService : IDisposable
    {
        public IConfiguration _configuration { get; }
        private readonly ApiConnector _apiConnector;
        private string _baseReqURL = "/api/ParamFormField";
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

        public ParamFormFieldService(IConfiguration configuration, ApiConnector apiConnector)
        {
            _configuration = configuration;
            _apiConnector = apiConnector;
        }

        public async Task<List<SetupBorangListModel>> ListAll()
        {
            var result = new List<SetupBorangListModel>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetList";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result =  JsonConvert.DeserializeObject<List<SetupBorangListModel>>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<SetupBorangListModel>();
            }

            return result;
        }

        public async Task<List<SetupBorangListModel>> ListByFormType(string formType)
        {
            var result = new List<SetupBorangListModel>();
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
                        result = JsonConvert.DeserializeObject<List<SetupBorangListModel>>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<SetupBorangListModel>();
            }

            return result;
        }

        public async Task<SetupBorangListModel> ViewDetail(int id)
        {
            var result = new SetupBorangListModel();
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
                        result = JsonConvert.DeserializeObject<SetupBorangListModel>(dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new SetupBorangListModel();
            }

            return result;
        }        
        public async Task<ReturnViewModel> Create(SetupBorangListModel inputModel)
        {

            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Create";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
            }

            return result;
        }


        public async Task<ReturnViewModel> Update(int id, SetupBorangListModel inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
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
