using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Text;

namespace PBTPro.Data
{
    public class UserService : IDisposable
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
        protected readonly AuditLogger _cf;
        private readonly ILogger<UserService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;

        private string _baseReqURL = "/api/User";
        private string _baseReqURLAuth = "/api/Authenticate";
        private string LoggerName = "";
        private List<user_profile_view> _user { get; set; }

        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
        }

        [HttpGet]
        public async Task<List<ApplicationUser>> ListAll()
        {
            var result = new List<ApplicationUser>();
            string requestUrl = $"{_baseReqURL}/GetListUser";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<ApplicationUser>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai tetapan bangsa.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<ApplicationUser>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<RegisterModel>> ListsAll()
        {
            var result = new List<RegisterModel>();
            string requestUrl = $"{_baseReqURL}/GetList";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<RegisterModel>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai tetapan bangsa.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<RegisterModel>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<RegisterModel>> Refresh()
        {
            var result = new List<RegisterModel>();
            try
            {
                string requestUrl = $"{_baseReqURL}/GetList";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<RegisterModel>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai tetapan bangsa.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<RegisterModel>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(RegisterModel inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                inputModel.Password = GeneratePassword();
                inputModel.Name = inputModel.FullName;
               
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                if (response.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "TIdak berjaya tambah data.", 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, RegisterModel inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                //RegisterModel rm = new RegisterModel();
                //rm.FullName = inputModel.full_name;
                //rm.ICNo = inputModel.IdNo;
                //rm.PhoneNo = inputModel.PhoneNumber;
                //rm.Email = inputModel.Email;
                //rm.DepartmentID = inputModel.dept_id;
                //rm.DivisionID = inputModel.div_id;
                //rm.UnitID = inputModel.unit_id;

                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel.Id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data.", 1, LoggerName, "");

            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Delete(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/Delete/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public Task<List<user_profile_view>> GetUserAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai tetapan bangsa.", 1, LoggerName, "");
            return Task.FromResult(_user);
        }

        public async Task<user_profile_view> ViewDetail(int id)
        {
            var result = new user_profile_view();
            try
            {
                string requestquery = $"/{id}";
                string requestUrl = $"{_baseReqURL}/ViewDetail{requestquery}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<user_profile_view>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new user_profile_view();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
        [HttpGet]
        public string GeneratePassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
            };

            Random rand = new Random(System.Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (opts.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (opts.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (opts.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        #region 
        //private List<system_user> _User { get; set; }
        //public IConfiguration _configuration { get; }
        //private readonly PBTProDbContext _dbContext;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //protected readonly CommonFunction _cf;
        //private readonly ILogger<UserService> _logger;
        //private string LoggerName = "";
        //string _controllerName = "";

        //public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, PBTProDbContext dbContext)
        //{
        //    _configuration = configuration;
        //    _dbContext = dbContext;
        //    _httpContextAccessor = httpContextAccessor;
        //    _cf = new CommonFunction(httpContextAccessor, configuration);
        //    _logger = logger;
        //    _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        //    CreateUserList();
        //}

        //public void GetDefaultPermission()
        //{
        //    if (LoggerName != null || LoggerName != "")
        //        LoggerName = "1";//User.Identity.Name;  // assign value to logger name
        //    else LoggerName = null;
        //}

        //public async void CreateUserList()
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

        //    try
        //    {
        //        //////var platformApiUrl = _configuration["PlatformAPI"];
        //        //////var accessToken = _cf.CheckToken();

        //        //////var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
        //        //////string jsonString = await _cf.List(request);

        //        ////////Open this when the API is completed
        //        //////_User = JsonConvert.DeserializeObject<List<system_user>>(jsonString);

        //        _User = new List<system_user> {
        //            new system_user {
        //                user_id = 1,
        //                user_name = "650721031221",
        //                full_name = "Azman Bin Alias",
        //                ic_no = "650721031221",
        //                mobile_no = "0123456789",
        //                user_email = "john_doe@gmail.com",
        //                dept_id = 1,
        //                dept_name = "Jabatan Penilaian",
        //                section_id = 1,
        //                section_name =  "Harta dan Pusaka",
        //                unit_id = 1,
        //                unit_name = "Unit 001",
        //                created_date = DateTime.Parse("2024/01/05")
        //            },
        //            new system_user {
        //                user_id = 2,
        //                user_name = "701121163223",
        //                full_name = "Abu Bakar Bin Jamal",
        //                ic_no = "701121163223",
        //                mobile_no = "0129876543",
        //                user_email = "azman610@gmail.com",
        //                dept_id = 1,
        //                dept_name = "Jabatan Penilaian",
        //                section_id = 1,
        //                section_name =  "Harta dan Pusaka",
        //                unit_id = 1,
        //                unit_name = "Unit 001",
        //                created_date = DateTime.Parse("2023/03/10")
        //            }
        //         };


        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna sistem.", Convert.ToInt32(uID), LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //    }
        //}

        //public Task<List<system_user>> GetUserAsync(CancellationToken ct = default)
        //{
        //    return Task.FromResult(_User);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<int> AddUser([FromBody] string strData = "")
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/User/InsertUser");
        //        string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/User/InsertUser");
        //        system_user dtData = JsonConvert.DeserializeObject<system_user>(jsonString);

        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah pengguna baru.", Convert.ToInt32(uID), LoggerName, "");
        //        return dtData.user_id;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return 0;
        //    }
        //}

        //[HttpDelete]
        //public async Task<bool> DeleteUser(int id)
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/User/DeleteUser/" + id);
        //        string jsonString = await _cf.Delete(request);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return false;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPut]
        //public async Task<int> UpdateUser(system_user dtData)
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var uri = platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id;
        //        var request = _cf.CheckRequestPut(platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id);
        //        string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //        return dtData.user_id;

        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return 0;
        //    }
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<List<system_user>> RefreshUserAsync()
        //{
        //    GetDefaultPermission();
        //    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
        //        string jsonString = await _cf.List(request);
        //        List<system_user> dtData = JsonConvert.DeserializeObject<List<system_user>>(jsonString);
        //        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //        return dtData;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //        return null;
        //    }
        //}

        #endregion


        //////private bool disposed = false;

        //////protected virtual void Dispose(bool disposing)
        //////{
        //////    if (!disposed)
        //////    {
        //////        if (disposing)
        //////        {
        //////            // Dispose managed resources here
        //////        }

        //////        // Dispose unmanaged resources here

        //////        disposed = true;
        //////    }
        //////}

        //////public void Dispose()
        //////{
        //////    Dispose(true);
        //////    GC.SuppressFinalize(this);
        //////}

        //////private List<system_user> _User { get; set; }
        //////public IConfiguration _configuration { get; }
        //////private readonly PBTProDbContext _dbContext;
        //////private readonly IHttpContextAccessor _httpContextAccessor;
        //////protected readonly CommonFunction _cf;
        //////private readonly ILogger<UserService> _logger;
        //////private string LoggerName = "";
        //////string _controllerName = "";

        //////public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, PBTProDbContext dbContext)
        //////{
        //////    _configuration = configuration;
        //////    _dbContext = dbContext;
        //////    _httpContextAccessor = httpContextAccessor;
        //////    _cf = new CommonFunction(httpContextAccessor, configuration);
        //////    _logger = logger;
        //////    _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        //////    CreateUserList();
        //////}

        //////public void GetDefaultPermission()
        //////{
        //////    if (LoggerName != null || LoggerName != "")
        //////        LoggerName = "1";//User.Identity.Name;  // assign value to logger name
        //////    else LoggerName = null;
        //////}

        //////public async void CreateUserList()
        //////{
        //////    GetDefaultPermission();
        //////    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

        //////    try
        //////    {
        //////        //////var platformApiUrl = _configuration["PlatformAPI"];
        //////        //////var accessToken = _cf.CheckToken();

        //////        //////var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
        //////        //////string jsonString = await _cf.List(request);

        //////        ////////Open this when the API is completed
        //////        //////_User = JsonConvert.DeserializeObject<List<system_user>>(jsonString);

        //////        _User = new List<system_user> {
        //////            new system_user {
        //////                user_id = 1,
        //////                user_name = "650721031221",
        //////                full_name = "Azman Bin Alias",
        //////                ic_no = "650721031221",
        //////                mobile_no = "0123456789",
        //////                user_email = "john_doe@gmail.com",
        //////                dept_id = 1,
        //////                dept_name = "Jabatan Penilaian",
        //////                div_id = 1,
        //////                div_name =  "Harta dan Pusaka",
        //////                unit_id = 1,
        //////                unit_name = "Unit 001",
        //////                created_date = DateTime.Parse("2024/01/05")
        //////            },
        //////            new system_user {
        //////                user_id = 2,
        //////                user_name = "701121163223",
        //////                full_name = "Abu Bakar Bin Jamal",
        //////                ic_no = "701121163223",
        //////                mobile_no = "0129876543",
        //////                user_email = "azman610@gmail.com",
        //////                dept_id = 1,
        //////                dept_name = "Jabatan Penilaian",
        //////                div_id = 1,
        //////                div_name =  "Harta dan Pusaka",
        //////                unit_id = 1,
        //////                unit_name = "Unit 001",
        //////                created_date = DateTime.Parse("2023/03/10")
        //////            }
        //////         };


        //////        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna sistem.", Convert.ToInt32(uID), LoggerName, "");
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //////    }
        //////}

        //////public Task<List<system_user>> GetUserAsync(CancellationToken ct = default)
        //////{
        //////    return Task.FromResult(_User);
        //////}

        //////[AllowAnonymous]
        //////[HttpPost]
        //////public async Task<int> AddUser([FromBody] string strData = "")
        //////{
        //////    GetDefaultPermission();
        //////    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //////    try
        //////    {
        //////        var platformApiUrl = _configuration["PlatformAPI"];
        //////        var accessToken = _cf.CheckToken();

        //////        var request = _cf.CheckRequest(platformApiUrl + "/api/User/InsertUser");
        //////        string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/User/InsertUser");
        //////        system_user dtData = JsonConvert.DeserializeObject<system_user>(jsonString);

        //////        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah pengguna baru.", Convert.ToInt32(uID), LoggerName, "");
        //////        return dtData.user_id;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //////        return 0;
        //////    }
        //////}

        //////[HttpDelete]
        //////public async Task<bool> DeleteUser(int id)
        //////{
        //////    GetDefaultPermission();
        //////    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //////    try
        //////    {
        //////        var platformApiUrl = _configuration["PlatformAPI"];
        //////        var accessToken = _cf.CheckToken();

        //////        var request = _cf.CheckRequest(platformApiUrl + "/api/User/DeleteUser/" + id);
        //////        string jsonString = await _cf.Delete(request);
        //////        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //////        return true;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //////        return false;
        //////    }
        //////}

        //////[AllowAnonymous]
        //////[HttpPut]
        //////public async Task<int> UpdateUser(system_user dtData)
        //////{
        //////    GetDefaultPermission();
        //////    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //////    try
        //////    {
        //////        var platformApiUrl = _configuration["PlatformAPI"];
        //////        var accessToken = _cf.CheckToken();

        //////        var uri = platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id;
        //////        var request = _cf.CheckRequestPut(platformApiUrl + "/api/User/UpdateUser/" + dtData.user_id);
        //////        string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
        //////        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //////        return dtData.user_id;

        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //////        return 0;
        //////    }
        //////}

        //////[AllowAnonymous]
        //////[HttpGet]
        //////public async Task<List<system_user>> RefreshUserAsync()
        //////{
        //////    GetDefaultPermission();
        //////    var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
        //////    try
        //////    {
        //////        var platformApiUrl = _configuration["PlatformAPI"];
        //////        var accessToken = _cf.CheckToken();

        //////        var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
        //////        string jsonString = await _cf.List(request);
        //////        List<system_user> dtData = JsonConvert.DeserializeObject<List<system_user>>(jsonString);
        //////        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna.", Convert.ToInt32(uID), LoggerName, "");

        //////        return dtData;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
        //////        return null;
        //////    }
        //////}



    }
}