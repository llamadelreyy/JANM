using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Text;

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

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AuditLogger _cf;
        private readonly ILogger<RoleMenuService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        private string _baseReqURL = "/api/RoleMenu";
        private string LoggerName = "";


        private List<role_menu> _RoleMenu { get; set; }
        
        public RoleMenuService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RoleMenuService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            CreateRoleMenu();
        }       

        public async void CreateRoleMenu()
        {
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
                                menu_id = 101,
                                menu_name = "Tetapan",
                                submenu_name = "Pengguna Sistem",
                                menu_path = "user_system",
                                header_id = 1,     //mark this is the menu with first child
                                parent_id = 100,
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
                                menu_id = 102,
                                menu_name = "Tetapan",
                                submenu_name = "Peranan Sistem",
                                menu_path = "role",
                                header_id = 1,
                                parent_id = 100,
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
                                menu_id = 103,
                                menu_name = "Tetapan",
                                submenu_name = "Pengguna & Peranan",
                                menu_path = "user_role",
                                header_id = 1,
                                parent_id = 100,
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
                                menu_id = 201,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Ringkasan Eksekutif",
                                menu_path = "zon_eksekutif",
                                header_id = 1,
                                parent_id = 200,
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
                                menu_id = 202,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Ringkasan Zon Majlis",
                                menu_path = "zon_majlis",
                                header_id = 1,
                                parent_id = 200,
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
                                menu_id = 203,
                                menu_name = "Papan Pemuka",
                                submenu_name = "Graf Statistik",
                                menu_path = "./",
                                header_id = 1,
                                parent_id = 200,
                                sort_order = 2,
                                icon_url =  "\\images\\icons-small\\chart-up-color.png",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = true,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 7,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 400,
                                menu_name = "Perincian Data",
                                submenu_name = "Perincian Data",
                                menu_path = "./",
                                header_id = 0,
                                parent_id = 0,
                                sort_order = 1,
                                icon_url =  "\\images\\icons-small\\blue-document-template.png",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = true,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            },
                            new role_menu {
                                _id = 8,
                                role_id = 1,
                                role_name = "Administrator",
                                menu_id = 500,
                                menu_name = "Nota Pemeriksaan",
                                submenu_name = "Nota Pemeriksaan",
                                menu_path = "./",
                                header_id = 0,
                                parent_id = 0,
                                sort_order = 1,
                                icon_url =  "\\images\\icons-small\\blog--pencil.png",
                                bln_create = false,
                                bln_update = false,
                                bln_delete = false,
                                bln_print = true,
                                bln_read = true,
                                created_date = DateTime.Parse("2023/03/11")
                            }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan menu sistem.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            finally
            {
            }
        }

        public Task<List<role_menu>> GetRoleMenuAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");
            return Task.FromResult(_RoleMenu);
        }

        [HttpGet]
        public async Task<List<role_menu>> ListAll()
        {
            var result = new List<role_menu>();
            string requestUrl = $"{_baseReqURL}/ListAll";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<role_menu>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<role_menu>();
            }
            return result;
        }

        [HttpGet]
        public async Task<List<role_menu>> Refresh()
        {
            var result = new List<role_menu>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListAll";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<role_menu>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<role_menu>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Add(role_menu inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Add";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya tambah data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        public async Task<ReturnViewModel> Update(int id, role_menu inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/Update/{inputModel._id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk jadual rondaan.", 1, LoggerName, "");

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
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk soalan lazim.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }

        //public Task<List<role_menu>> GetSectionAsync(CancellationToken ct = default)
        //{
        //    var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk soalan lazim.", 1, LoggerName, "");
        //    return Task.FromResult(_Section);
        //}

        public async Task<role_menu> ViewDetail(int id)
        {
            var result = new role_menu();
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
                        result = JsonConvert.DeserializeObject<role_menu>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci soalan lazim.", 1, LoggerName, "");
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                result = new role_menu();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
            return result;
        }
    }
}
//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<ActionResult<role_menu>> InsertRoleMenu([FromBody] string strData = "")
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/InsertRoleMenu");
//                string jsonString = await _cf.AddNew(request, strData, platformApiUrl + "/api/RoleMenu/InsertRoleMenu");
//                role_menu dtData = JsonConvert.DeserializeObject<role_menu>(jsonString);

//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah peranan menu baru.", Convert.ToInt32(uID), LoggerName, "");
//                return dtData;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }

//        [HttpDelete]
//        public async Task<int> DeleteRoleMenu(int id)
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/DeleteRoleMenu/" + id);
//                string jsonString = await _cf.Delete(request);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data peranan menu.", Convert.ToInt32(uID), LoggerName, "");

//                return id;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return 0;
//            }
//        }

//        [AllowAnonymous]
//        [HttpPut]
//        public async Task<ActionResult<role_menu>> UpdateRoleMenu(int id, role_menu dtData)
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var uri = platformApiUrl + "/api/RoleMenu/UpdateRoleMenu/" + id;
//                var request = _cf.CheckRequestPut(platformApiUrl + "/api/RoleMenu/UpdateRoleMenu/" + id);
//                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(dtData), uri);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk peranan menu.", Convert.ToInt32(uID), LoggerName, "");

//                return dtData;

//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<List<role_menu>> RefreshRoleMenuAsync()
//        {
//            GetDefaultPermission();
//            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
//            try
//            {
//                var platformApiUrl = _configuration["PlatformAPI"];
//                var accessToken = _cf.CheckToken();

//                var request = _cf.CheckRequest(platformApiUrl + "/api/RoleMenu/ListRoleMenu");
//                string jsonString = await _cf.List(request);
//                List<role_menu> dtData = JsonConvert.DeserializeObject<List<role_menu>>(jsonString);
//                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar senarai peranan menu.", Convert.ToInt32(uID), LoggerName, "");

//                return dtData;
//            }
//            catch (Exception ex)
//            {
//                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
//                return null;
//            }
//        }

//    }
//}
