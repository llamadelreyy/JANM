using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpo.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBT.Pages;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBT.Services
{
    [AllowAnonymous]
    public partial class FaqService : IDisposable
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
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<FaqService> _logger;

        public FaqService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<FaqService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<TbFaq>> GetAllFaq()
        {
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/ListFaq");
                string jsonString = await _cf.List(request);

                List<TbFaq> faqList = JsonConvert.DeserializeObject<List<TbFaq>>(jsonString);
                return faqList;
            }
            catch (Exception ex)
            {
                //TempData.Clear();
                //ViewData["OperationMsg"] = ex.Message;
                //_cf.SystemLoggerAsync("AddNewFaq", "", uID, LoggerName, "", 0, ex.Message, 0, "MasjidKita");
                return null;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<TbFaq> GetIdFaq(int id)
        {
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/RetrieveFaq/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);

                TbFaq faq = JsonConvert.DeserializeObject<TbFaq>(jsonString.ToString());

                return faq;
            }
            catch (Exception ex) 
            {
                // _cf.SystemLoggerAsync("SaveEditPackage", "", uID, LoggerName, "", 0, ex.Message);
                return null;
            }
        }

        private TbFaq NotFoundResult()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<TbFaq> AddNewFaq(string strFaq)
        {

            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/InsertFaq");
                string jsonString = await _cf.AddNew(request, strFaq, platformApiUrl + "/api/Faq/InsertFaq");
                TbFaq faq = JsonConvert.DeserializeObject<TbFaq>(jsonString);
                //_cf.SystemLogAsync(false, false, true, LoggerName, "AddNewFaq", "", uID, LoggerName, "", 0, "Soalan lazim dihantar ke pangkalan data", 0, "MasjidKita");

                return faq;
            }
            catch (Exception ex)
            {
                // _cf.SystemLoggerAsync("SaveEditPackage", "", uID, LoggerName, "", 0, ex.Message);
                return null;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/DeleteFaq/" + id);
                string jsonString = await _cf.Delete(request);
                return Ok(jsonString);
            }
            catch (Exception ex) 
            {
                // _cf.SystemLoggerAsync("SaveEditPackage", "", uID, LoggerName, "", 0, ex.Message);
                return null;
            }
        }

        private IActionResult Ok(string jsonString)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<TbFaq>> PostFaq([FromBody] string faqs = "")
        {
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Faq/InsertFaq");
                string jsonString = await _cf.AddNew(request, faqs, platformApiUrl + "/api/Faq/InsertFaq");
                TbFaq faq = JsonConvert.DeserializeObject<TbFaq>(jsonString);

                //_cf.SystemLogAsync(false, false, true, LoggerName, "AddNewFaq", "", uID, LoggerName, "", 0, "Soalan lazim dihantar ke pangkalan data", 0, "MasjidKita");

                return faq;
            }
            catch (Exception ex)
            {
                //TempData.Clear();
                //ViewData["OperationMsg"] = ex.Message;
                //_cf.SystemLoggerAsync("AddNewFaq", "", uID, LoggerName, "", 0, ex.Message, 0, "MasjidKita");
                return null;
            }
        }
        
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> PutFaq(int id, TbFaq faq)
        {
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Faq/UpdateFaq/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Faq/UpdateFaq/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(faq), uri);
                return Ok(jsonString);

            }
            catch (Exception ex)
            {
               // _cf.SystemLoggerAsync("SaveEditPackage", "", uID, LoggerName, "", 0, ex.Message);
                return null;
            }
        }
    }
}
