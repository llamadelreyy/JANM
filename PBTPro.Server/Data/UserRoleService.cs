using MySqlConnector;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Data;
using System.Reflection;

namespace PBTPro.Data
{
    public class UserRoleService : IDisposable
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

        private List<UserRoleProp> _UserRole { get; set; }

        public IConfiguration _configuration { get; }
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<UserRoleService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public UserRoleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserRoleService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateUserRole();
        }
        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }
        public async void CreateUserRole()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {

                //////var platformApiUrl = _configuration["PlatformAPI"];
                //////var accessToken = _cf.CheckToken();

                //////var request = _cf.CheckRequest(platformApiUrl + "/api/UserRole/ListUserRole");
                //////string jsonString = await _cf.List(request);

                ////////Open this when the API is completed
                //////_UserRole = JsonConvert.DeserializeObject<List<UserRoleProp>>(jsonString);
                ///

                _UserRole = new List<UserRoleProp> {
                    new UserRoleProp {
                        table_id = 1,
                        user_id = 1,
                        role_id = 1,
                        user_name = "mbdk240015",
                        role_name = "Administrator",
                        role_desc = "Admin of the system",
                        user_full_name = "Azman Bin Alias",
                        created_date = DateTime.Parse("2024/01/05")
                    },
                    new UserRoleProp {
                        table_id = 2,
                        user_id = 2,
                        role_id = 1,
                        user_name = "mbdk230010",
                        role_name = "Administrator",
                        role_desc = "Admin of the system",
                        user_full_name = "Abu Bakar Bin Jamal",
                        created_date = DateTime.Parse("2023/03/10")
                    },
                    new UserRoleProp {
                        table_id = 3,
                        user_id = 2,
                        role_id = 2,
                        user_name = "mbdk230010",
                        role_name = "Head of Department",
                        role_desc = "Head of department for perlesenan",
                        user_full_name = "Abu Bakar Bin Jamal",
                        created_date = DateTime.Parse("2023/03/10")
                    }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai pengguna dan peranan sistem.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }

        }

        public Task<List<UserRoleProp>> GetUserRoleAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_UserRole);
        }

    }
}