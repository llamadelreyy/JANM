using System.Data;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using System.Reflection;
using PBTPro.DAL.Services;
using PBTPro.DAL.Models.CommonServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PBTPro.Data
{
    public class RoleMenuService : IDisposable
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
        private List<role_menu> _RoleMenu { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<RoleMenuService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public RoleMenuService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleMenuService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
            CreateRoleMenu();
        }

        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        public async void CreateRoleMenu()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            try
            {

                //////var platformApiUrl = _configuration["PlatformAPI"];
                //////var accessToken = _cf.CheckToken();

                //////var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/ListRoleMenu");
                //////string jsonString = await _cf.List(request);

                ////////Open this when the API is completed
                //////_Role = JsonConvert.DeserializeObject<List<role_menu>>(jsonString);
                ///

                _RoleMenu = new List<role_menu> {
                            new role_menu {
                                _id = 1,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 1,
                                menu_name = "Tetapan",
                                submenu_name = "Pengguna Sistem",
                                menu_path = "user_system",
                                header_id = 1,     //mark this is the menu with first child
                                parent_id = 1,
                                sort_order = 0,
                                icon_url =  "",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = false,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 2,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 2,
                                menu_name = "Tetapan",
                                submenu_name = "Peranan Sistem",
                                menu_path = "role",
                                header_id = 0,
                                parent_id = 1,
                                sort_order = 1,
                                icon_url =  "\\images\\icons-small\\xfn.png",
                                bln_create = true,
                                bln_update = true,
                                bln_delete = true,
                                bln_print = false,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 3,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 3,
                                menu_name = "Tetapan",
                                submenu_name = "Pengguna & Peranan",
                                menu_path = "user_role",
                                header_id = 0,
                                parent_id = 1,
                                sort_order = 2,
                                icon_url =  "\\images\\icons-small\\toolbox.png",
                                bln_create = true,
                                bln_update = true,
                                bln_delete = true,
                                bln_print = false,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 4,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 4,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Ringkasan Eksekutif",
                                menu_path = "zon_eksekutif",
                                header_id = 2,
                                parent_id = 2,
                                sort_order = 0,
                                icon_url =  "",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = false,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 5,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 5,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Ringkasan Zon Majlis",
                                menu_path = "zon_majlis",
                                header_id = 0,
                                parent_id = 2,
                                sort_order = 1,
                                icon_url =  "\\images\\icons-small\\table-heatmap.png",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = true,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 6,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 6,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Graf Statistik",
                                menu_path = "./",
                                header_id = 0,
                                parent_id = 2,
                                sort_order = 2,
                                icon_url =  "\\images\\icons-small\\chart-up-color.png",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = true,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan menu sistem.", Convert.ToInt32(uID), LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
            }
            finally
            {
            }
        }

        public Task<List<role_menu>> GetRoleMenuAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_RoleMenu);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<role_menu>> InsertRoleMenu([FromBody] string strData = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/InsertRoleMenu");
                string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/RoleMenu/InsertRoleMenu");
                role_menu dtData = JsonConvert.DeserializeObject<role_menu>(jsonString);

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah peranan menu baru.", Convert.ToInt32(uID), LoggerName, "");
                return dtData;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteRoleMenu(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/DeleteRoleMenu/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data peranan menu.", Convert.ToInt32(uID), LoggerName, "");

                return id;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return 0;
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult<role_menu>> UpdateRoleMenu(int id, role_menu dtData)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/RoleMenu/UpdateRoleMenu/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/RoleMenu/UpdateRoleMenu/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk peranan menu.", Convert.ToInt32(uID), LoggerName, "");

                return dtData;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<role_menu>> RefreshRoleMenuAsync()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/ListRoleMenu");
                string jsonString = await _cf.List(request);
                List<role_menu> dtData = JsonConvert.DeserializeObject<List<role_menu>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan menu.", Convert.ToInt32(uID), LoggerName, "");

                return dtData;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

    }
}
