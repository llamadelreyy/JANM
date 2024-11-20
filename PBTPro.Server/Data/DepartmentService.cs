/*
Project: PBT Pro
Description: Department service to handle department Form Field
Author: Nurulfarhana
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Services;
using System.Reflection;

namespace PBTPro.Data
{
    [AllowAnonymous]
    public partial class DepartmentService : IDisposable
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
        private List<department_info> _Department { get; set; }

        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CommonFunction _cf;
        protected readonly SharedFunction _sf;
        private readonly ILogger<DepartmentService> _logger;
        private string LoggerName = "";
        string _controllerName = "";

        public DepartmentService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<DepartmentService> logger, PBTProDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cf = new CommonFunction(httpContextAccessor, configuration);
            _sf = new SharedFunction(httpContextAccessor);
            _logger = logger;
            _controllerName = (string)(_httpContextAccessor.HttpContext?.Request.RouteValues["controller"]);
        }
        public void GetDefaultPermission()
        {
            if (LoggerName != null || LoggerName != "")
                LoggerName = "1";//User.Identity.Name;  // assign value to logger name
            else LoggerName = null;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<department_info>> GetAllDepartment()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Department/ListDepartment");
                string jsonString = await _cf.List(request);
                List<department_info> departmentList = JsonConvert.DeserializeObject<List<department_info>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai jabatan.", Convert.ToInt32(uID), LoggerName, "");

                return departmentList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<department_info>> RefreshListDepartment()
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Department/ListDepartment");
                string jsonString = await _cf.List(request);
                List<department_info> departmentList = JsonConvert.DeserializeObject<List<department_info>>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai jabatan.", Convert.ToInt32(uID), LoggerName, "");

                return departmentList;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<department_info> GetIdDepartment(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Department/RetrieveDepartment/" + id);
                string jsonString = await _cf.Retrieve(request, accessToken);
                department_info departmentProp = JsonConvert.DeserializeObject<department_info>(jsonString.ToString());
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar maklumat terperinci jabatan.", Convert.ToInt32(uID), LoggerName, "");

                return departmentProp;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<department_info>> PostDepartment([FromBody] string departments = "")
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Department/InsertDepartment");
                string jsonString = await _cf.AddNew(request, departments, platformApiUrl + "/api/Department/InsertDepartment");
                department_info departmentProp = JsonConvert.DeserializeObject<department_info>(jsonString);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Tambah data baru untuk jabatan.", Convert.ToInt32(uID), LoggerName, "");

                return departmentProp;
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }

        [HttpDelete]
        public async Task<int> DeleteDepartment(int id)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var request = _cf.CheckRequest(platformApiUrl + "/api/Department/DeleteDepartment/" + id);
                string jsonString = await _cf.Delete(request);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data untuk jabatan.", Convert.ToInt32(uID), LoggerName, "");

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
        public async Task<ActionResult<department_info>> PutDepartment(int id, department_info department)
        {
            GetDefaultPermission();
            var uID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            try
            {
                var platformApiUrl = _configuration["PlatformAPI"];
                var accessToken = _cf.CheckToken();

                var uri = platformApiUrl + "/api/Department/UpdateDepartment/" + id;
                var request = _cf.CheckRequestPut(platformApiUrl + "/api/Department/UpdateDepartment/" + id);
                string jsonString = await _cf.Update(request, JsonConvert.SerializeObject(department), uri);
                await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data untuk jabatan.", Convert.ToInt32(uID), LoggerName, "");

                return department;

            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, Convert.ToInt32(uID), LoggerName, "");
                return null;
            }
        }
        public Task<List<department_info>> GetDepartmentAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai untuk jabatan.", 1, LoggerName, "");
            return Task.FromResult(_Department);
        }
    }
}
