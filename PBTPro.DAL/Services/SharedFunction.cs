using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Services
{
    public class SharedFunction : ControllerBase
    {
        PBTProDbContext _dbContext;
        protected readonly string? _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IWebHostEnvironment _environment;
        private string LoggerName = "";
        string uID = "";
        private string ApiURL = "";
        //protected readonly CommonFunction _cf;

        public SharedFunction(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
            //_cf = new CommonFunction(contextAccessor);
            //uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        }
        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
        
    }
}
