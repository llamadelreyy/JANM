/*
Project: PBT Pro
Description: AppMarking API controller to handle app marking from mobile
Author: Fakhrul
Date: July 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AppMarkingController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly string _feature = "APP_MARKING";
        private readonly ILogger<AppMarkingController> _logger;
        private readonly long _maxImageFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };
        private readonly int _defCRS = 4326;

        public AppMarkingController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<AppMarkingController> logger, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, string? filterType, int? crs = null)
        {
            try
            {
                IQueryable<trn_premis> initQuery = _tenantDBContext.trn_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs == null || crs == _defCRS)
                {
                    crs = _defCRS;
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
                }
                else
                {
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)));
                }

                var resultData = await initQuery.Where(x => x.codeid_premis != null)
                .Select(x => new app_marking_marker
                {
                    codeid_premis = x.codeid_premis,
                    lot = x.lot,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom))
                })
                .ToListAsync();

                if (!resultData.Any())
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }


                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data marker berjaya dijana")));

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<trn_notice>>> ListAll()
        {
            try
            {
                var data = await _tenantDBContext.trn_notices.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetail(string CodeidPremis)
        {
            try
            {
                var result = await _tenantDBContext.trn_premis
                .Where(x => x.codeid_premis == CodeidPremis)
                .GroupJoin(
                    _tenantDBContext.trn_licensees,
                    premis => premis.codeid_premis,
                    license => license.codeid_premis,
                    (premis, licenses) => new { premis, licenses }
                )
                .Select(g => new app_marking_view
                {
                    id = g.premis.id,
                    codeid_premis = g.premis.codeid_premis,
                    owner_name = g.premis.owner_name,
                    owner_icno = g.premis.owner_icno,
                    owner_phone_no = g.premis.owner_telno,
                    owner_address = g.premis.owner_addr,
                    tax_acc_no = g.premis.tax_accno,
                    tax_status = g.premis.tax_status_id,
                    lot = g.premis.lot,
                    address = g.premis.address,
                    notes = g.premis.notes,
                    category = g.premis.category,
                    image = g.premis.image,
                    latitude = g.premis.geom != null ? g.premis.geom.Y : (double?)null,
                    longitude = g.premis.geom != null ? g.premis.geom.X : (double?)null,
                    status = g.premis.status,

                    licensees = g.licenses.Any()
                        ? g.licenses.Select(lic => new app_marking_license
                        {
                            id = lic.id,
                            codeid_premis = lic.codeid_premis,
                            owner_name = lic.owner_name,
                            owner_icno = lic.owner_icno,
                            owner_phone_no = g.premis.owner_telno,
                            owner_address = g.premis.owner_addr,
                            ssm_no = lic.ssm_no,
                            business_name = lic.business_name,
                            business_address = lic.business_addr,
                            license_acc_no = lic.license_accno,
                            license_type = lic.cat_id,
                            license_status = lic.license_status_id,
                            activity = lic.activity,
                            floor = lic.floor,
                            notes = lic.notes,
                            pic_name = lic.pic_name,
                            pic_phone_no = lic.pic_phone_no,
                            image_1 = lic.image_1,
                            image_2 = lic.image_2,
                            image_3 = lic.image_3,
                            image_4 = lic.image_4,
                            image_5 = lic.image_5,
                            image_6 = lic.image_6,
                            status = lic.status
                        }).ToList()
                        : null
                })
                .FirstOrDefaultAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPremis([FromForm] app_marking_tax_input_model InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Start Transaction
                using (var transaction = await _tenantDBContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        string timestamp = DateTime.Now.ToString("yyMMdd_HHmmssfff");
                        string codeid_premis = $"APMMK-{timestamp}";
                        #region Main Data

                        Point? geom = null;
                        if (InputModel.latitude.HasValue && InputModel.longitude.HasValue)
                        {
                            geom = new Point(InputModel.longitude.Value, InputModel.latitude.Value) { SRID = 4326 };
                        }

                        trn_premis premis = new trn_premis
                        {
                            owner_name = InputModel.owner_name,
                            owner_icno = InputModel.owner_icno,
                            owner_telno = InputModel.owner_phone_no,
                            owner_addr = InputModel.owner_address,
                            tax_accno = InputModel.tax_acc_no,
                            tax_status_id = InputModel.tax_status,
                            notes = InputModel.tax_notes,
                            lot = InputModel.tax_lot_no,
                            category = InputModel.tax_category,
                            address = InputModel.tax_address,
                            geom = geom,
                            codeid_premis = codeid_premis,
                            is_deleted = false,
                            creator_id = runUserID,
                            created_at = DateTime.Now,
                        };
                        #endregion

                        #region image
                        if (InputModel.tax_image != null)
                        {
                            string ImageUploadExt = Path.GetExtension(InputModel.tax_image.FileName).ToString().ToLower();
                            string Filename = $"{codeid_premis}_image_p{ImageUploadExt}";
                            var UploadPath = await getUploadPath();
                            var Fullpath = Path.Combine(UploadPath, Filename);
                            using (var stream = new FileStream(Fullpath, FileMode.Create))
                            {
                                await InputModel.tax_image.CopyToAsync(stream);
                            }
                            string pathurl = await getViewUrl();
                            premis.image = $"{pathurl}/{Filename}";
                        }
                        #endregion

                        _tenantDBContext.trn_premis.Add(premis);
                        await _tenantDBContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        var result = new app_marking_tax
                        {
                            id = premis.id,
                            owner_name = premis.owner_name,
                            owner_icno = premis.owner_icno,
                            owner_phone_no = premis.owner_telno,
                            owner_address = premis.owner_addr,
                            tax_acc_no = premis.tax_accno,
                            tax_status = premis.tax_status_id,
                            notes = premis.notes,
                            lot = InputModel.tax_lot_no,
                            category = premis.category,
                            address = premis.address,
                            latitude = InputModel.latitude,
                            longitude = InputModel.longitude,
                            codeid_premis = premis.codeid_premis,
                            status = premis.status
                        };

                        return Ok(result, SystemMesg(_feature, "CREATE_PREMIS", MessageTypeEnum.Success, string.Format("Berjaya cipta premis")));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLicense([FromForm] app_marking_license_input_model InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Start Transaction
                using (var transaction = await _tenantDBContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        string timestamp = DateTime.Now.ToString("yyMMdd_HHmmssfff");
                        string codeid_premis = $"APMMK-{timestamp}";
                        #region Main Data
                        trn_licensee license = new trn_licensee
                        {
                            codeid_premis = InputModel.codeid_premis,
                            owner_name = InputModel.owner_name,
                            owner_icno = InputModel.owner_icno,
                            owner_telno = InputModel.owner_phone_no,
                            owner_addr = InputModel.owner_address,
                            ssm_no = InputModel.ssm_no,
                            business_name = InputModel.business_name,
                            business_addr = InputModel.business_address,
                            license_accno = InputModel.license_acc_no,
                            license_status_id = InputModel.license_status,
                            floor = InputModel.license_floor,
                            pic_name = InputModel.license_pic_name,
                            pic_phone_no = InputModel.license_pic_phone_no,
                            activity = InputModel.license_activity,
                            notes = InputModel.license_notes,
                            cat_id = InputModel.license_type,
                            is_deleted = false,
                            creator_id = runUserID,
                            created_at = DateTime.Now,
                        };
                        #endregion

                        #region image
                        if (InputModel.license_images != null && InputModel.license_images.Count > 0)
                        {
                            int pfn = 0;
                            string pathurl = await getViewUrl();
                            foreach (var ip in InputModel.license_images)
                            {
                                pfn++;
                                string ImageUploadExt = Path.GetExtension(ip.FileName).ToString().ToLower();
                                string Filename = $"{codeid_premis}_image_{pfn}{ImageUploadExt}";

                                var UploadPath = await getUploadPath();
                                var Fullpath = Path.Combine(UploadPath, Filename);
                                using (var stream = new FileStream(Fullpath, FileMode.Create))
                                {
                                    await ip.CopyToAsync(stream);
                                }

                                typeof(trn_licensee).GetProperty($"image_{pfn}")
                                    ?.SetValue(license, $"{pathurl}/{Filename}");
                            }

                        }
                        #endregion
                        _tenantDBContext.trn_licensees.Add(license);
                        await _tenantDBContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Ok(license, SystemMesg(_feature, "CREATE_LICENSE", MessageTypeEnum.Success, string.Format("Berjaya cipta lesen")));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Private Logic
        private async Task<string?> getUploadPath()
        {
            string? result;
            result = await getBaseUploadPath("app_marking");
            return result;
        }

        private async Task<string?> getViewUrl()
        {
            string? result;
            result = await getBaseViewUrl("app_marking");
            return result;
        }

        private async Task<string?> getBaseUploadPath(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "BaseUploadPath").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();
                //result = "/Users/sirmael/data/var/www/PBTPro/wwwroot/tenants";
                string? tenantId = _tenantId.ToString();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    result = Path.Combine(result, tenantId);
                }

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

        private async Task<string?> getBaseViewUrl(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "Tenant" && x.param_name == "ImageViewUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();
                //result = "http://192.168.1.2/tenants";
                string? tenantId = _tenantId.ToString();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    result = result + "/" + tenantId;
                }

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
