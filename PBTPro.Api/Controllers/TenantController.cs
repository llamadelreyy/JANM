/*
Project: PBT Pro
Description: RefLawAct controller
Author: Ismail
Date: February 2025
Version: 1.0

Additional Notes:
- 

Changes Logs:
04/02/2025 - initial create
*/
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Globalization;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class TenantController : IBaseController
    {
        private readonly ILogger<TenantController> _logger;
        private readonly string _feature = "TENANT";
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };
        private readonly long _maxDocumentFileSize = 15 * 1024 * 1024;
        private readonly List<string> _documentFileExt = new List<string> { ".doc", ".docx", ".pdf" };

        public TenantController(PBTProDbContext dbContext, ILogger<TenantController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue) {
                    tenantId = await getDefTenantId();
                }

                var UserId = await getDefRunUserId();
                tenant_profile_view data = new tenant_profile_view();

                var baseImageViewURL = await getViewUrl(tenantId.ToString(),"images");
                var AvatarViewURL = baseImageViewURL;

                var TenantProfile = await _dbContext.tenants.Where(x => x.tenant_id == tenantId)
                .Select(x => new tenant_profile_view
                {
                   tenant_id = x.tenant_id,
                   tn_name = x.tn_name,
                   addr_line1 = x.addr_line1,
                   addr_line2 = x.addr_line2,
                   town_code = x.town_code,
                   district_code = x.district_code,
                   state_code = x.state_code,
                   country_code = x.country_code,
                   postcode = x.postcode,
                   tn_photo_url = !string.IsNullOrWhiteSpace(x.tn_photo_filename) ? AvatarViewURL + "/" + x.tn_photo_filename : null
                }).AsNoTracking().FirstOrDefaultAsync();

                return Ok(TenantProfile, SystemMesg(_feature, "VIEW_TENANT_PROFILE", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] update_tenant_profile_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var tenant = await _dbContext.tenants.FirstOrDefaultAsync(x => x.tenant_id == InputModel.tenant_id);
                if (tenant == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (InputModel.tn_photo_file != null)
                {
                    if (!IsFileExtensionAllowed(InputModel.tn_photo_file, _imageFileExt))
                    {
                        var imageFileExtString = String.Join(", ", _imageFileExt.ToList());
                        List<string> param = new List<string> { imageFileExtString };
                        return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong ([0])."), param));
                    }

                    if (!IsFileSizeWithinLimit(InputModel.tn_photo_file, _maxImageFileSize))
                    {
                        List<string> param = new List<string> { FormatFileSize(_maxImageFileSize) };
                        return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                    }
                }
                #endregion

                string? FileName = tenant.tn_photo_filename;
                if (InputModel.tn_photo_file != null && InputModel.tn_photo_file?.Length > 0)
                {
                    string ImageUploadExt = Path.GetExtension(InputModel.tn_photo_file.FileName).ToString().ToLower();

                    FileName = $"tenant_logo{ImageUploadExt}";
                    var UploadPath = await getUploadPath(tenant.tenant_id.ToString(),"images");
                    var Fullpath = Path.Combine(UploadPath, FileName);
                    using (var stream = new FileStream(Fullpath, FileMode.Create))
                    {
                        await InputModel.tn_photo_file.CopyToAsync(stream);
                    }

                    tenant.tn_photo_filename = FileName;
                }

                tenant.tn_name = InputModel.tn_name;
                tenant.addr_line1 = InputModel.addr_line1;
                tenant.addr_line2 = InputModel.addr_line2;
                tenant.town_code = InputModel.town_code;
                tenant.district_code = InputModel.district_code;
                tenant.state_code = InputModel.state_code;
                tenant.country_code = InputModel.country_code;
                tenant.postcode = InputModel.postcode;
                tenant.modifier_id = runUserID;
                tenant.modified_at = DateTime.Now;

                _dbContext.tenants.Update(tenant);
                await _dbContext.SaveChangesAsync();

                return Ok("", SystemMesg(_feature, "UPDATE_PROFILE", MessageTypeEnum.Success, string.Format("profil berjaya disimpan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region standard CRUD
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var tenants = await _dbContext.tenants.Where(x => x.is_deleted != true).AsNoTracking().ToListAsync();

                if (tenants.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(tenants, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var tenant = await _dbContext.tenants.FirstOrDefaultAsync(x => x.tenant_id == Id);

                if (tenant == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(tenant, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] tenant InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                if (string.IsNullOrEmpty(InputModel.tn_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan name diperlukan")));
                }
                #endregion

                tenant tenant = new tenant
                {
                    tn_name = InputModel.tn_name,
                    tn_email = InputModel.tn_email,
                    tn_photo_filename = InputModel.tn_photo_filename,
                    tn_photo_url = InputModel.tn_photo_url,
                    tn_doc_filename = InputModel.tn_doc_filename,
                    tn_doc_url = InputModel.tn_doc_url,
                    addr_line1 = InputModel.addr_line1,
                    addr_line2 = InputModel.addr_line2,
                    town_code = InputModel.town_code,
                    district_code = InputModel.district_code,
                    state_code = InputModel.state_code,
                    country_code = InputModel.country_code,
                    postcode = InputModel.postcode,
                    phone_number = InputModel.phone_number,
                    contact_name = InputModel.contact_name,
                    accept_term1 = InputModel.accept_term1,
                    accept_term2 = InputModel.accept_term2,
                    accept_term3 = InputModel.accept_term3,
                    accept_term4 = InputModel.accept_term4,
                    site_name = InputModel.site_name,
                    connection_string = InputModel.connection_string,
                    schema_name = InputModel.schema_name,
                    table_prefix = InputModel.table_prefix,
                    recipe_name = InputModel.recipe_name,
                    tn_handle = InputModel.tn_handle,
                    url_prefix = InputModel.url_prefix,
                    website_link = InputModel.website_link,
                    confirm_website_link = InputModel.confirm_website_link,
                    website_status_id = InputModel.website_status_id,
                    subsc_status_id = InputModel.subsc_status_id,
                    subsc_start_date = InputModel.subsc_start_date,
                    subsc_end_date = InputModel.subsc_end_date,
                    reminder_date = InputModel.reminder_date,
                    creator_id = runUserID,
                    created_at = DateTime.Now
                };

                _dbContext.tenants.Add(tenant);
                await _dbContext.SaveChangesAsync();
                return Ok(tenant, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah penyewa")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] tenant InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var tenant = await _dbContext.tenants.FirstOrDefaultAsync(x => x.tenant_id == Id);
                if (tenant == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrEmpty(InputModel.tn_name))
                {
                    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan nama diperlukan")));
                }
                #endregion

                tenant.tn_name = InputModel.tn_name;
                tenant.tn_email = InputModel.tn_email;
                tenant.tn_photo_filename = InputModel.tn_photo_filename;
                tenant.tn_photo_url = InputModel.tn_photo_url;
                tenant.tn_doc_filename = InputModel.tn_doc_filename;
                tenant.tn_doc_url = InputModel.tn_doc_url;
                tenant.addr_line1 = InputModel.addr_line1;
                tenant.addr_line2 = InputModel.addr_line2;
                tenant.town_code = InputModel.town_code;
                tenant.district_code = InputModel.district_code;
                tenant.state_code = InputModel.state_code;
                tenant.country_code = InputModel.country_code;
                tenant.postcode = InputModel.postcode;
                tenant.phone_number = InputModel.phone_number;
                tenant.contact_name = InputModel.contact_name;
                tenant.accept_term1 = InputModel.accept_term1;
                tenant.accept_term2 = InputModel.accept_term2;
                tenant.accept_term3 = InputModel.accept_term3;
                tenant.accept_term4 = InputModel.accept_term4;
                tenant.site_name = InputModel.site_name;
                tenant.connection_string = InputModel.connection_string;
                tenant.schema_name = InputModel.schema_name;
                tenant.table_prefix = InputModel.table_prefix;
                tenant.recipe_name = InputModel.recipe_name;
                tenant.tn_handle = InputModel.tn_handle;
                tenant.url_prefix = InputModel.url_prefix;
                tenant.website_link = InputModel.website_link;
                tenant.confirm_website_link = InputModel.confirm_website_link;
                tenant.website_status_id = InputModel.website_status_id;
                tenant.subsc_status_id = InputModel.subsc_status_id;
                tenant.subsc_start_date = InputModel.subsc_start_date;
                tenant.subsc_end_date = InputModel.subsc_end_date;
                tenant.reminder_date = InputModel.reminder_date;
                tenant.modifier_id = runUserID;
                tenant.modified_at = DateTime.Now;

                _dbContext.tenants.Update(tenant);
                await _dbContext.SaveChangesAsync();

                return Ok(tenant, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai penyewa")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Remove(int Id)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                #region Validation
                var tenant = await _dbContext.tenants.FirstOrDefaultAsync(x => x.tenant_id == Id);
                if (tenant == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                try
                {
                    _dbContext.tenants.Remove(tenant);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    tenant.is_deleted = true;
                    tenant.modifier_id = runUserID;
                    tenant.modified_at = DateTime.Now;

                    _dbContext.tenants.Update(tenant);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(tenant, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang penyewa")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        #region Private Logic
        protected async Task<string?> getUploadPath(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "BaseUploadPath").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = Path.Combine(result, lv1);
                    if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = Path.Combine(result, lv2);
                        if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = Path.Combine(result, lv3);
                            if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = Path.Combine(result, lv4);
                                if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
                            }
                        }
                    }
                }
            }
            return result;
        }

        protected async Task<string?> getViewUrl(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "ImageViewUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = result + "/" + lv1;
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = result + "/" + lv2;
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = result + "/" + lv3;
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = result + "/" + lv4;
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion
    }
}


