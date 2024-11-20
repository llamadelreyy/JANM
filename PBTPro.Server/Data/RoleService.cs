using MySqlConnector;
using System.Data;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using System.Reflection;
using PBTPro.DAL.Services;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Data
{
    public class RoleService : IDisposable
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
        private List<RoleProp> _Role { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<RoleService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public RoleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateRole();
        }

        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        public async void CreateRole()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {

                //////var platformApiUrl = _configuration["PlatformAPI"];
                //////var accessToken = _cf.CheckToken();

                //////var request = _cf.CheckRequest(platformApiUrl + "/api/User/ListUser");
                //////string jsonString = await _cf.List(request);

                ////////Open this when the API is completed
                //////_Role = JsonConvert.DeserializeObject<List<RoleProp>>(jsonString);
                ///

                _Role = new List<RoleProp> {
                        new RoleProp {
                            role_id = 1,
                            role_name = "Administrator",
                            role_desc = "Admin of the system",
                            created_date = DateTime.Parse("2023/03/11")
                        },
                        new RoleProp {
                            role_id = 2,
                            role_name = "Head of Department",
                            role_desc = "Head of department for perlesenan",
                            created_date = DateTime.Parse("2023/03/11")
                        }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan sistem.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }
            finally
            {
            }
        }

        public Task<List<RoleProp>> GetRoleAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Role);
        }




    }
}