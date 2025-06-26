/*
Project: PBT Pro
Description: sample for premis marker point 
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
22/11/2024 - initial create
26/11/2024 - change all hardcoded sql query to EF function
30/01/2025 (Author: Fakhrul) - added GetFilteredListByBound function where it will return the status value for both lesen and cukai based on traffic light priority
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Linq;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremisController : IBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PremisController> _logger;
        private readonly string _feature = "PREMIS";
        private readonly int _defCRS = 4326;

        public PremisController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<PremisController> logger) : base(dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _tenantDBContext = tntdbContext;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList(int? crs = null)
        {
            try
            {
                IQueryable<mst_premis> initQuery = _tenantDBContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs != null && crs == _defCRS)
                {
                    initQuery = initQuery
                        .Select(x => new mst_premis { codeid_premis = x.codeid_premis, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var queryWithJoin = from premis in initQuery
                                    join tax in _tenantDBContext.mst_license_premis_taxes
                                    on premis.codeid_premis equals tax.codeid_premis into taxGroup
                                    from tax in taxGroup.DefaultIfEmpty()
                                    join licStatus in _tenantDBContext.ref_license_statuses
                                    on (tax != null ? tax.status_lesen_id : (int?)null) equals licStatus.status_id into licStatusGroup
                                    from licStatus in licStatusGroup.DefaultIfEmpty()
                                    join taxStatus in _tenantDBContext.ref_tax_statuses
                                    on (tax != null ? tax.status_tax_id : (int?)null) equals taxStatus.status_id into taxStatusGroup
                                    from taxStatus in taxStatusGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        Premis = premis,
                                        jnLicTax = tax,
                                        licStatus = licStatus,
                                        taxStatus = taxStatus
                                    };

                var mst_premis = await queryWithJoin.Where(x => x.jnLicTax != null)
                .Select(x => new PremisMarkerViewModel
                {
                    codeid_premis = x.Premis.codeid_premis,
                    lot = x.Premis.lot,
                    category = x.Premis.category,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.Premis.geom)),
                    license_status_id = x.jnLicTax.status_lesen_id,
                    license_status_view = x.licStatus.status_name,
                    tax_status_id = x.jnLicTax.status_tax_id,
                    tax_status_view = x.taxStatus.status_name
                })
                .ToListAsync();


                if (mst_premis.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                //mst_lots = await _dbContext.mst_lots.AsNoTracking().ToListAsync();
                return Ok(mst_premis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetListByBound")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, string? filterType, int? crs = null)
        {
            try
            {
                IQueryable<mst_premis> initQuery = _tenantDBContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs == null || crs == _defCRS)
                {
                    crs = _defCRS;
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
                }
                else
                {
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
                        .Select(x => new mst_premis { codeid_premis = x.codeid_premis, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var queryWithJoin = from premis in initQuery
                                    join tax in _tenantDBContext.mst_license_premis_taxes
                                    on premis.codeid_premis equals tax.codeid_premis into taxGroup
                                    from tax in taxGroup.DefaultIfEmpty()
                                    join licStatus in _tenantDBContext.ref_license_statuses
                                    on (tax != null ? tax.status_lesen_id : (int?)null) equals licStatus.status_id into licStatusGroup
                                    from licStatus in licStatusGroup.DefaultIfEmpty()
                                    join taxStatus in _tenantDBContext.ref_tax_statuses
                                    on (tax != null ? tax.status_tax_id : (int?)null) equals taxStatus.status_id into taxStatusGroup
                                    from taxStatus in taxStatusGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        Premis = premis,
                                        jnLicTax = tax,
                                        licStatus = licStatus,
                                        taxStatus = taxStatus
                                    };

                var mst_premis = await queryWithJoin.Where(x => x.jnLicTax != null)
                .OrderBy(x => x.licStatus.priority == x.taxStatus.priority ? x.licStatus.priority : Math.Min(x.licStatus.priority, x.taxStatus.priority))
                .ThenBy(x => x.licStatus.priority == x.taxStatus.priority ? 0 : (x.licStatus.priority < x.taxStatus.priority ? 0 : 1))
                .Select(x => new PremisMarkerViewModel
                {
                    codeid_premis = x.Premis.codeid_premis,
                    lot = x.Premis.lot,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.Premis.geom)),
                    license_status_id = x.jnLicTax.status_lesen_id,
                    license_status_view = x.licStatus.status_name,
                    tax_status_id = x.jnLicTax.status_tax_id,
                    tax_status_view = x.taxStatus.status_name
                })
                .ToListAsync();

                // comment error mobile 216
                //List<String> ft = JsonConvert.DeserializeObject<List<String>>(filterType);

                if (filterType != null && filterType.Any())
                {
                    List<string> ft = filterType.Split(',').ToList();
                    mst_premis = mst_premis.Where(x =>
                                    filterType.Contains(Convert.ToString(x.license_status_view)) ||
                                    filterType.Contains(Convert.ToString(x.tax_status_view))
                                ).ToList();
                }

                if (mst_premis.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                mst_premis = mst_premis.DistinctBy(premis => premis.codeid_premis).ToList();
                //mst_lots = await _dbContext.mst_lots.AsNoTracking().ToListAsync();
                return Ok(mst_premis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetFilteredListByBound")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFilteredListByBound(double minLng, double minLat, double maxLng, double maxLat, string? filterType, int? crs = null)
        {
            try
            {
                // Retrieve initial list of mst_premis
                IQueryable<mst_premis> initQuery = _tenantDBContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs == null || crs == _defCRS)
                {
                    crs = _defCRS;
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
                }
                else
                {
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
                        .Select(x => new mst_premis { codeid_premis = x.codeid_premis, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var queryWithJoin = from premis in initQuery
                                    join tax in _tenantDBContext.mst_license_premis_taxes
                                    on premis.codeid_premis equals tax.codeid_premis into taxGroup
                                    from tax in taxGroup.DefaultIfEmpty()
                                    join licStatus in _tenantDBContext.ref_license_statuses
                                    on (tax != null ? tax.status_lesen_id : (int?)null) equals licStatus.status_id into licStatusGroup
                                    from licStatus in licStatusGroup.DefaultIfEmpty()
                                    join taxStatus in _tenantDBContext.ref_tax_statuses
                                    on (tax != null ? tax.status_tax_id : (int?)null) equals taxStatus.status_id into taxStatusGroup
                                    from taxStatus in taxStatusGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        Premis = premis,
                                        jnLicTax = tax,
                                        licStatus = licStatus,
                                        taxStatus = taxStatus
                                    };
                List<string> statusFilters = new List<string>();
                if (!string.IsNullOrEmpty(filterType))
                {
                    statusFilters = filterType.Split(',')
                                                .Select(f => f.Trim().ToLower())
                                                .ToList();

                    if (statusFilters.Count > 0)
                    {
                        //queryWithJoin = queryWithJoin
                        //.Where(x => statusFilters.Any(filter =>
                        //x.licStatus.status_name.ToLower().Contains(filter.ToLower()) ||
                        //x.taxStatus.status_name.ToLower().Contains(filter.ToLower())
                        //));
                        queryWithJoin = queryWithJoin
                        .Where(x => statusFilters.Any(filter =>
                            filter == "ltd"
                                ? x.licStatus.status_name.ToLower().Contains("tiada data")
                                : filter == "ctd"
                                    ? x.taxStatus.status_name.ToLower().Contains("tiada data")
                                    : x.licStatus.status_name.ToLower().Contains(filter) ||
                                      x.taxStatus.status_name.ToLower().Contains(filter)
                        ));
                    }
                }
                else
                {
                    queryWithJoin = queryWithJoin
                    .OrderBy(x => x.licStatus.priority == x.taxStatus.priority ? x.licStatus.priority : Math.Min(x.licStatus.priority, x.taxStatus.priority))
                    .ThenBy(x => x.licStatus.priority == x.taxStatus.priority ? 0 : (x.licStatus.priority < x.taxStatus.priority ? 0 : 1));
                }

                var mst_premisList = await queryWithJoin.Where(x => x.jnLicTax != null)
                .Select(x => new
                {
                    codeid_premis = x.Premis.codeid_premis,
                    lot = x.Premis.lot,
                    category = x.Premis.category,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.Premis.geom)),
                    license_status_id = x.jnLicTax.status_lesen_id,
                    license_status_view = x.licStatus.status_name,
                    license_status_color = x.licStatus.color,
                    license_status_priority = x.licStatus.priority,
                    tax_status_id = x.jnLicTax.status_tax_id,
                    tax_status_view = x.taxStatus.status_name,
                    tax_status_color = x.taxStatus.color,
                    tax_status_priority = x.taxStatus.priority
                })
                .ToListAsync();

                // Check if any records were found
                if (!mst_premisList.Any())
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                var resultData = new List<premis_marker>();
                foreach (var premis in mst_premisList.DistinctBy(premis => premis.codeid_premis).ToList())
                {
                    String marker_cukai_status = premis.tax_status_view;
                    String marker_lesen_status = premis.license_status_view;
                    String marker_color = premis.license_status_priority <= premis.tax_status_priority ? premis.license_status_color : premis.tax_status_color;

                    if (statusFilters.Count > 0)
                    {
                        if (statusFilters.Any(filter => premis.tax_status_view.ToLower().Contains(filter.ToLower())))
                        {
                            marker_color = premis.tax_status_color;
                        }
                        if (statusFilters.Any(filter => premis.license_status_view.ToLower().Contains(filter.ToLower())))
                        {
                            marker_color = premis.license_status_color;
                        }
                    }

                    resultData.Add(new premis_marker
                    {
                        codeid_premis = premis.codeid_premis,
                        category = premis.category,
                        lot = premis.lot,
                        marker_cukai_status = marker_cukai_status,
                        marker_lesen_status = marker_lesen_status,
                        marker_color = marker_color,
                        geom = premis.geom
                    });
                }

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetFilteredListByBoundWeb")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFilteredListByBoundWeb(double minLng, double minLat, double maxLng, double maxLat, string? filterType, string? viewType, int? crs = null)
        {
            try
            {
                // Retrieve initial list of mst_premis
                IQueryable<mst_premis> initQuery = _tenantDBContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs == null || crs == _defCRS)
                {
                    crs = _defCRS;
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
                }
                else
                {
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
                        .Select(x => new mst_premis { codeid_premis = x.codeid_premis, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var queryWithJoin = from premis in initQuery
                                    join tax in _tenantDBContext.mst_license_premis_taxes
                                    on premis.codeid_premis equals tax.codeid_premis into taxGroup
                                    from tax in taxGroup.DefaultIfEmpty()
                                    join licStatus in _tenantDBContext.ref_license_statuses
                                    on (tax != null ? tax.status_lesen_id : (int?)null) equals licStatus.status_id into licStatusGroup
                                    from licStatus in licStatusGroup.DefaultIfEmpty()
                                    join taxStatus in _tenantDBContext.ref_tax_statuses
                                    on (tax != null ? tax.status_tax_id : (int?)null) equals taxStatus.status_id into taxStatusGroup
                                    from taxStatus in taxStatusGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        Premis = premis,
                                        jnLicTax = tax,
                                        licStatus = licStatus,
                                        taxStatus = taxStatus
                                    };
                List<string> statusFilters = new List<string>();
                if (!string.IsNullOrEmpty(filterType))
                {
                    statusFilters = filterType.Split(',')
                                                .Select(f => f.Trim().ToLower())
                                                .ToList();

                    if (statusFilters.Count > 0)
                    {
                        //queryWithJoin = queryWithJoin
                        //.Where(x => statusFilters.Any(filter =>
                        //x.licStatus.status_name.ToLower().Contains(filter.ToLower()) ||
                        //x.taxStatus.status_name.ToLower().Contains(filter.ToLower())
                        //));
                        queryWithJoin = queryWithJoin
                        .Where(x => statusFilters.Any(filter =>
                            filter == "ltd"
                                ? x.licStatus.status_name.ToLower().Contains("tiada data")
                                : filter == "ctd"
                                    ? x.taxStatus.status_name.ToLower().Contains("tiada data")
                                    : x.licStatus.status_name.ToLower().Contains(filter) ||
                                      x.taxStatus.status_name.ToLower().Contains(filter)
                        ));
                    }
                }
                else
                {
                    queryWithJoin = queryWithJoin
                    .OrderBy(x => x.licStatus.priority == x.taxStatus.priority ? x.licStatus.priority : Math.Min(x.licStatus.priority, x.taxStatus.priority))
                    .ThenBy(x => x.licStatus.priority == x.taxStatus.priority ? 0 : (x.licStatus.priority < x.taxStatus.priority ? 0 : 1));
                }

                var mst_premisList = await queryWithJoin.Where(x => x.jnLicTax != null)
                .Select(x => new
                {
                    codeid_premis = x.Premis.codeid_premis,
                    lot = x.Premis.lot,
                    category = x.Premis.category,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.Premis.geom)),
                    license_status_id = x.jnLicTax.status_lesen_id,
                    license_status_view = x.licStatus.status_name,
                    license_status_color = x.licStatus.color,
                    license_status_priority = x.licStatus.priority,
                    tax_status_id = x.jnLicTax.status_tax_id,
                    tax_status_view = x.taxStatus.status_name,
                    tax_status_color = x.taxStatus.color,
                    tax_status_priority = x.taxStatus.priority
                })
                .ToListAsync();

                // Check if any records were found
                if (!mst_premisList.Any())
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                var resultData = new List<premis_marker_web>();
                int total_lesen_aktif = mst_premisList?.Count(x => x.license_status_id == 1) ?? 0;
                int total_lesen_tamat_tempoh = mst_premisList?.Count(x => x.license_status_id == 2) ?? 0;
                int total_lesen_gantung = mst_premisList?.Count(x => x.license_status_id == 3) ?? 0;
                int total_lesen_tiada_data = mst_premisList?.Count(x => x.license_status_id == 4) ?? 0;
                int total_lesen_tidak_berlesen = mst_premisList?.Count(x => x.license_status_id == 5) ?? 0;
                int total_cukai_dibayar = mst_premisList?.Count(x => x.tax_status_id == 7) ?? 0;
                int total_cukai_tertungak = mst_premisList?.Count(x => x.tax_status_id == 8) ?? 0;
                int total_cukai_tiada_data = mst_premisList?.Count(x => x.tax_status_id == 9) ?? 0;
                foreach (var premis in mst_premisList.DistinctBy(premis => premis.codeid_premis).ToList())
                {
                    String marker_cukai_status = premis.tax_status_view;
                    String marker_lesen_status = premis.license_status_view;
                    String marker_color = premis.license_status_priority <= premis.tax_status_priority ? premis.license_status_color : premis.tax_status_color;

                    if (!string.IsNullOrWhiteSpace(viewType) && viewType.ToLower() == "license")
                    {
                        marker_color = premis.license_status_color;
                    }
                    if (!string.IsNullOrWhiteSpace(viewType) && viewType.ToLower() == "tax")
                    {
                        marker_color = premis.tax_status_color;
                    }

                    if (statusFilters.Count > 0)
                    {
                        if (statusFilters.Any(filter => premis.tax_status_view.ToLower().Contains(filter.ToLower())))
                        {
                            marker_color = premis.tax_status_color;
                        }
                        if (statusFilters.Any(filter => premis.license_status_view.ToLower().Contains(filter.ToLower())))
                        {
                            marker_color = premis.license_status_color;
                        }
                    }

                    resultData.Add(new premis_marker_web
                    {
                        codeid_premis = premis.codeid_premis,
                        lot = premis.lot,
                        category = premis.category,
                        marker_cukai_status = marker_cukai_status,
                        marker_lesen_status = marker_lesen_status,
                        marker_color = marker_color,
                        geom = premis.geom,
                        total_lesen_aktif = total_lesen_aktif,
                        total_lesen_tamat_tempoh = total_lesen_tamat_tempoh,
                        total_lesen_gantung = total_lesen_gantung,
                        total_lesen_tiada_data = total_lesen_tiada_data,
                        total_lesen_tidak_berlesen = total_lesen_tidak_berlesen,
                        total_cukai_dibayar = total_cukai_dibayar,
                        total_cukai_tertungak = total_cukai_tertungak,
                        total_cukai_tiada_data = total_cukai_tiada_data
                    });
                }

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetPremisInfo")]
        public async Task<IActionResult> GetPremisInfo(string codeid)
        {
            try
            {
                var premisInfo = await _tenantDBContext.mst_premis.Where(x => x.codeid_premis == codeid).AsNoTracking().FirstOrDefaultAsync();
                if (premisInfo == null)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                var queryWithJoin = from mlpt in _tenantDBContext.mst_license_premis_taxes
                                    where mlpt.codeid_premis == premisInfo.codeid_premis
                                    join license in _tenantDBContext.mst_licensees
                                    on mlpt.licensee_id equals license.licensee_id into licenseGroup
                                    from license in licenseGroup.DefaultIfEmpty()
                                    join licOwner in _tenantDBContext.mst_owner_licensees
                                    on license.owner_icno equals licOwner.owner_icno into licOwnerGroup
                                    from licOwner in licOwnerGroup.DefaultIfEmpty()
                                    join tax in _tenantDBContext.mst_taxholders
                                    on mlpt.taxholder_id equals tax.taxholder_id into taxGroup
                                    from tax in taxGroup.DefaultIfEmpty()
                                    join taxOwner in _tenantDBContext.mst_owner_premis
                                    on tax.owner_icno equals taxOwner.owner_icno into taxOwnerGroup
                                    from taxOwner in taxOwnerGroup.DefaultIfEmpty()
                                    join licStatus in _tenantDBContext.ref_license_statuses
                                    on mlpt.status_lesen_id equals licStatus.status_id into licStatusGroup
                                    from licStatus in licStatusGroup.DefaultIfEmpty()
                                    join taxStatus in _tenantDBContext.ref_tax_statuses
                                    on mlpt.status_tax_id equals taxStatus.status_id into taxStatusGroup
                                    from taxStatus in taxStatusGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        jnLicTax = mlpt,
                                        license = license,
                                        licOwner = licOwner,
                                        licStatus = licStatus,
                                        tax = tax,
                                        taxOwner = taxOwner,
                                        taxStatus = taxStatus
                                    };

                var rawJDs = await queryWithJoin.AsNoTracking().ToListAsync();

                premis_view result = new premis_view();
                result = (premis_view)premisInfo;
                result.premis_license_tax = new List<premis_license_tax_view>();

                foreach (var rawJD in rawJDs
                .OrderBy(x => string.IsNullOrEmpty(x.jnLicTax.floor_building) ? 1 : (char.IsLetter(x.jnLicTax.floor_building.Trim().FirstOrDefault()) ? 0 : 1))
                //.ThenBy(x => new string(x.jnLicTax.floor_building.TakeWhile(char.IsLetter).ToArray()))
                //.ThenBy(x => GetNumericPart(x.jnLicTax.floor_building))
                //.OrderBy(x => x.jnLicTax.floor_building)
                .ThenBy(x => x.jnLicTax.license_premis_tax_id)
                )
                {
                    var joinData = (premis_license_tax_view)rawJD.jnLicTax;
                    joinData.license = rawJD.license;
                    joinData.license_owner = rawJD.licOwner;
                    joinData.license_status = rawJD.licStatus;
                    joinData.tax = rawJD.tax;
                    joinData.tax_owner = rawJD.taxOwner;
                    joinData.tax_status = rawJD.taxStatus;
                    result.premis_license_tax.Add(joinData);
                }

                if (result == null)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        [Route("GetListByLicenseType")]
        public async Task<IActionResult> GetListByLicenseType([FromBody] PremisLicenseFilterModel filterModel, int? maxResult = null, int? page = 1)
        {
            try
            {
                IQueryable<view_premis_detail> initQuery = _tenantDBContext.view_premis_details;
                initQuery = initQuery.Where(x => x.license_type.StartsWith($"{filterModel.types_code}:") || x.license_type.Contains($"|{filterModel.types_code}:"));

                if (filterModel.start_date.HasValue)
                {
                    initQuery = initQuery.Where(x => x.license_start_date >= filterModel.start_date);
                }

                if (filterModel.end_date.HasValue)
                {
                    initQuery = initQuery.Where(x => x.license_end_date <= filterModel.end_date);
                }

                if (filterModel.category_ids?.Count > 0)
                {
                    initQuery = initQuery.Where(x => filterModel.category_ids.Contains(x.license_cat_id.Value));
                }

                #region Building Pagination
                var pageInfo = new PaginationInfo();
                if (maxResult.HasValue || maxResult > 0)
                {
                    var totalCount = await initQuery.CountAsync();
                    int totalPages = (int)Math.Ceiling(totalCount / (double)maxResult.Value);
                    pageInfo.TotalPages = totalPages;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = maxResult!.Value;
                    pageInfo.CurrentPageNo = page!.Value;
                    initQuery = initQuery
                        .Skip((page.Value - 1) * maxResult.Value)
                        .Take(maxResult.Value);
                }
                else
                {
                    var totalCount = await initQuery.CountAsync();
                    pageInfo.TotalPages = 1;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = totalCount;
                    pageInfo.CurrentPageNo = 1;
                }
                #endregion

                var results = await initQuery.Select(x => new general_search_premis_detail
                {
                    //primary key
                    codeid_premis = x.codeid_premis,
                    taxholder_id = x.taxholder_id,
                    tax_accno = x.tax_accno,
                    license_id = x.license_id,
                    license_accno = x.license_accno,
                    //premis data
                    premis_floor = x.premis_floor,
                    premis_lot = x.premis_lot,
                    premis_gkeseluruh = x.premis_gkeseluruh,
                    premis_longitude = PostGISFunctions.ST_X(x.premis_geom),
                    premis_latitude = PostGISFunctions.ST_Y(x.premis_geom),
                    premis_category = x.premis_category,
                    //tax data
                    tax_status_id = x.tax_status_id,
                    tax_status_view = x.tax_status_view,
                    tax_state_code = x.tax_state_code,
                    tax_district_code = x.tax_district_code,
                    tax_town_id = x.tax_town_id,
                    tax_parliment_id = x.tax_parliment_id,
                    tax_dun_id = x.tax_dun_id,
                    tax_zon_id = x.tax_zon_id,
                    tax_address = x.tax_address,
                    tax_start_date = x.tax_start_date,
                    tax_end_date = x.tax_end_date,
                    tax_owner_icno = x.tax_owner_icno,
                    tax_owner_name = x.tax_owner_name,
                    tax_owner_email = x.tax_owner_email,
                    tax_owner_telno = x.tax_owner_telno,
                    tax_owner_state_code = x.tax_owner_state_code,
                    tax_owner_disctict_code = x.tax_owner_disctict_code,
                    tax_owner_town_id = x.tax_owner_town_id,
                    tax_owner_addess = x.tax_owner_addess,
                    //license data
                    license_status_id = x.license_status_id,
                    license_status_view = x.license_status_view,
                    license_ssmno = x.license_ssmno,
                    license_business_name = x.license_business_name,
                    license_business_address = x.license_business_address,
                    license_state_code = x.license_state_code,
                    license_district_code = x.license_district_code,
                    license_town_id = x.license_town_id,
                    license_mukim_id = x.license_mukim_id,
                    license_lot = x.license_lot,
                    license_duration = x.license_duration,
                    license_cat_id = x.license_cat_id,
                    license_type = x.license_type,
                    license_total_amount = x.license_total_amount,
                    license_start_date = x.license_start_date,
                    license_end_date = x.license_end_date,
                    license_total_signboard = x.license_total_signboard,
                    license_signboard_size = x.license_signboard_size,
                    license_doc_support = x.license_doc_support,
                    license_g_activity_1 = x.license_g_activity_1,
                    license_g_activity_2 = x.license_g_activity_2,
                    license_g_activity_3 = x.license_g_activity_3,
                    license_g_signbboard_1 = x.license_g_signbboard_1,
                    license_g_signbboard_2 = x.license_g_signbboard_2,
                    license_g_signbboard_3 = x.license_g_signbboard_3,
                    license_owner_icno = x.license_owner_icno,
                    license_owner_name = x.license_owner_name,
                    license_owner_email = x.license_owner_email,
                    license_owner_telno = x.license_owner_telno,
                    license_owner_state_code = x.license_owner_state_code,
                    license_owner_disctict_code = x.license_owner_disctict_code,
                    license_owner_town_id = x.license_owner_town_id,
                    license_owner_addess = x.license_owner_addess
                }).ToListAsync();
                return Ok(results, pageInfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetListByDun")]
        public async Task<IActionResult> GetListByDun(int dun_id, int? maxResult = null, int? page = 1)
        {
            try
            {
                IQueryable<view_premis_detail> initQuery = _tenantDBContext.view_premis_details;
                initQuery = initQuery.Where(x => x.tax_dun_id == dun_id);

                #region Building Pagination
                var pageInfo = new PaginationInfo();
                if (maxResult.HasValue || maxResult > 0)
                {
                    var totalCount = await initQuery.CountAsync();
                    int totalPages = (int)Math.Ceiling(totalCount / (double)maxResult.Value);
                    pageInfo.TotalPages = totalPages;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = maxResult!.Value;
                    pageInfo.CurrentPageNo = page!.Value;
                    initQuery = initQuery
                        .Skip((page.Value - 1) * maxResult.Value)
                        .Take(maxResult.Value);
                }
                else
                {
                    var totalCount = await initQuery.CountAsync();
                    pageInfo.TotalPages = 1;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = totalCount;
                    pageInfo.CurrentPageNo = 1;
                }
                #endregion

                var results = await initQuery.Select(x => new general_search_premis_detail
                {
                    //primary key
                    codeid_premis = x.codeid_premis,
                    taxholder_id = x.taxholder_id,
                    tax_accno = x.tax_accno,
                    license_id = x.license_id,
                    license_accno = x.license_accno,
                    //premis data
                    premis_floor = x.premis_floor,
                    premis_lot = x.premis_lot,
                    premis_gkeseluruh = x.premis_gkeseluruh,
                    premis_longitude = PostGISFunctions.ST_X(x.premis_geom),
                    premis_latitude = PostGISFunctions.ST_Y(x.premis_geom),
                    premis_category = x.premis_category,
                    //tax data
                    tax_status_id = x.tax_status_id,
                    tax_status_view = x.tax_status_view,
                    tax_state_code = x.tax_state_code,
                    tax_district_code = x.tax_district_code,
                    tax_town_id = x.tax_town_id,
                    tax_parliment_id = x.tax_parliment_id,
                    tax_dun_id = x.tax_dun_id,
                    tax_zon_id = x.tax_zon_id,
                    tax_address = x.tax_address,
                    tax_start_date = x.tax_start_date,
                    tax_end_date = x.tax_end_date,
                    tax_owner_icno = x.tax_owner_icno,
                    tax_owner_name = x.tax_owner_name,
                    tax_owner_email = x.tax_owner_email,
                    tax_owner_telno = x.tax_owner_telno,
                    tax_owner_state_code = x.tax_owner_state_code,
                    tax_owner_disctict_code = x.tax_owner_disctict_code,
                    tax_owner_town_id = x.tax_owner_town_id,
                    tax_owner_addess = x.tax_owner_addess,
                    //license data
                    license_status_id = x.license_status_id,
                    license_status_view = x.license_status_view,
                    license_ssmno = x.license_ssmno,
                    license_business_name = x.license_business_name,
                    license_business_address = x.license_business_address,
                    license_state_code = x.license_state_code,
                    license_district_code = x.license_district_code,
                    license_town_id = x.license_town_id,
                    license_mukim_id = x.license_mukim_id,
                    license_lot = x.license_lot,
                    license_duration = x.license_duration,
                    license_cat_id = x.license_cat_id,
                    license_type = x.license_type,
                    license_total_amount = x.license_total_amount,
                    license_start_date = x.license_start_date,
                    license_end_date = x.license_end_date,
                    license_total_signboard = x.license_total_signboard,
                    license_signboard_size = x.license_signboard_size,
                    license_doc_support = x.license_doc_support,
                    license_g_activity_1 = x.license_g_activity_1,
                    license_g_activity_2 = x.license_g_activity_2,
                    license_g_activity_3 = x.license_g_activity_3,
                    license_g_signbboard_1 = x.license_g_signbboard_1,
                    license_g_signbboard_2 = x.license_g_signbboard_2,
                    license_g_signbboard_3 = x.license_g_signbboard_3,
                    license_owner_icno = x.license_owner_icno,
                    license_owner_name = x.license_owner_name,
                    license_owner_email = x.license_owner_email,
                    license_owner_telno = x.license_owner_telno,
                    license_owner_state_code = x.license_owner_state_code,
                    license_owner_disctict_code = x.license_owner_disctict_code,
                    license_owner_town_id = x.license_owner_town_id,
                    license_owner_addess = x.license_owner_addess
                }).ToListAsync();
                return Ok(results, pageInfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetListByParliment")]
        public async Task<IActionResult> GetListByParliment(int parliment_id, int? maxResult = null, int? page = 1)
        {
            try
            {
                IQueryable<view_premis_detail> initQuery = _tenantDBContext.view_premis_details;
                initQuery = initQuery.Where(x => x.tax_parliment_id == parliment_id);

                #region Building Pagination
                var pageInfo = new PaginationInfo();
                if (maxResult.HasValue || maxResult > 0)
                {
                    var totalCount = await initQuery.CountAsync();
                    int totalPages = (int)Math.Ceiling(totalCount / (double)maxResult.Value);
                    pageInfo.TotalPages = totalPages;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = maxResult!.Value;
                    pageInfo.CurrentPageNo = page!.Value;
                    initQuery = initQuery
                        .Skip((page.Value - 1) * maxResult.Value)
                        .Take(maxResult.Value);
                }
                else
                {
                    var totalCount = await initQuery.CountAsync();
                    pageInfo.TotalPages = 1;
                    pageInfo.TotalRecords = totalCount;
                    pageInfo.RecordPerPage = totalCount;
                    pageInfo.CurrentPageNo = 1;
                }
                #endregion

                var results = await initQuery.Select(x => new general_search_premis_detail
                {
                    //primary key
                    codeid_premis = x.codeid_premis,
                    taxholder_id = x.taxholder_id,
                    tax_accno = x.tax_accno,
                    license_id = x.license_id,
                    license_accno = x.license_accno,
                    //premis data
                    premis_floor = x.premis_floor,
                    premis_lot = x.premis_lot,
                    premis_gkeseluruh = x.premis_gkeseluruh,
                    premis_longitude = PostGISFunctions.ST_X(x.premis_geom),
                    premis_latitude = PostGISFunctions.ST_Y(x.premis_geom),
                    premis_category = x.premis_category,
                    //tax data
                    tax_status_id = x.tax_status_id,
                    tax_status_view = x.tax_status_view,
                    tax_state_code = x.tax_state_code,
                    tax_district_code = x.tax_district_code,
                    tax_town_id = x.tax_town_id,
                    tax_parliment_id = x.tax_parliment_id,
                    tax_dun_id = x.tax_dun_id,
                    tax_zon_id = x.tax_zon_id,
                    tax_address = x.tax_address,
                    tax_start_date = x.tax_start_date,
                    tax_end_date = x.tax_end_date,
                    tax_owner_icno = x.tax_owner_icno,
                    tax_owner_name = x.tax_owner_name,
                    tax_owner_email = x.tax_owner_email,
                    tax_owner_telno = x.tax_owner_telno,
                    tax_owner_state_code = x.tax_owner_state_code,
                    tax_owner_disctict_code = x.tax_owner_disctict_code,
                    tax_owner_town_id = x.tax_owner_town_id,
                    tax_owner_addess = x.tax_owner_addess,
                    //license data
                    license_status_id = x.license_status_id,
                    license_status_view = x.license_status_view,
                    license_ssmno = x.license_ssmno,
                    license_business_name = x.license_business_name,
                    license_business_address = x.license_business_address,
                    license_state_code = x.license_state_code,
                    license_district_code = x.license_district_code,
                    license_town_id = x.license_town_id,
                    license_mukim_id = x.license_mukim_id,
                    license_lot = x.license_lot,
                    license_duration = x.license_duration,
                    license_cat_id = x.license_cat_id,
                    license_type = x.license_type,
                    license_total_amount = x.license_total_amount,
                    license_start_date = x.license_start_date,
                    license_end_date = x.license_end_date,
                    license_total_signboard = x.license_total_signboard,
                    license_signboard_size = x.license_signboard_size,
                    license_doc_support = x.license_doc_support,
                    license_g_activity_1 = x.license_g_activity_1,
                    license_g_activity_2 = x.license_g_activity_2,
                    license_g_activity_3 = x.license_g_activity_3,
                    license_g_signbboard_1 = x.license_g_signbboard_1,
                    license_g_signbboard_2 = x.license_g_signbboard_2,
                    license_g_signbboard_3 = x.license_g_signbboard_3,
                    license_owner_icno = x.license_owner_icno,
                    license_owner_name = x.license_owner_name,
                    license_owner_email = x.license_owner_email,
                    license_owner_telno = x.license_owner_telno,
                    license_owner_state_code = x.license_owner_state_code,
                    license_owner_disctict_code = x.license_owner_disctict_code,
                    license_owner_town_id = x.license_owner_town_id,
                    license_owner_addess = x.license_owner_addess
                }).ToListAsync();
                return Ok(results, pageInfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region OldLogic

        //[HttpGet]
        //[Route("GetList")]
        //public async Task<IActionResult> GetList(int? crs = null)
        //{
        //    try
        //    {

        //        IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

        //        if (crs != null && crs == _defCRS)
        //        {
        //            initQuery = initQuery
        //                .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
        //        }

        //        var mst_premis = await initQuery
        //        .Select(x => new PremisMarkerViewModel
        //        {
        //            gid = x.gid,
        //            lot = x.lot,
        //            status_cukai = x.tempoh_sah_cukai == null
        //                        ? "None"
        //                        : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",
        //            status_lesen = x.tempoh_sah_lesen == null
        //                        ? "None"
        //                        : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",

        //            geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
        //        })
        //        .ToListAsync();


        //        if (mst_premis.Count == 0)
        //        {
        //            return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
        //        }

        //        //mst_lots = await _dbContext.mst_lots.AsNoTracking().ToListAsync();
        //        return Ok(mst_premis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        //[HttpGet]
        //[Route("GetListByBound")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, string? filterType, int? crs = null)
        //{
        //    try
        //    {
        //        IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

        //        if (crs == null || crs == _defCRS)
        //        {
        //            crs = _defCRS;
        //            initQuery = initQuery
        //                .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
        //        }
        //        else
        //        {
        //            initQuery = initQuery
        //                .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
        //                .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
        //        }



        //        var mst_premis = await initQuery
        //        .Select(x => new PremisMarkerViewModel
        //        {
        //            gid = x.gid,
        //            lot = x.lot,
        //            status_cukai = x.tempoh_sah_cukai == null
        //                        ? "None"
        //                        : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",
        //            status_lesen = x.tempoh_sah_lesen == null
        //                        ? "None"
        //                        : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",

        //            geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
        //        })
        //        .ToListAsync();

        //        // comment error mobile 216
        //        //List<String> ft = JsonConvert.DeserializeObject<List<String>>(filterType);

        //        if (filterType != null && filterType.Any())
        //        {
        //            List<string> ft = filterType.Split(',').ToList();
        //            mst_premis = mst_premis.Where(x =>
        //                            filterType.Contains(Convert.ToString(x.status_lesen)) ||
        //                            filterType.Contains(Convert.ToString(x.status_cukai))
        //                        ).ToList();
        //        }

        //        if (mst_premis.Count == 0)
        //        {
        //            return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
        //        }

        //        //mst_lots = await _dbContext.mst_lots.AsNoTracking().ToListAsync();
        //        return Ok(mst_premis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}


        //[HttpGet]
        //[Route("GetPremisInfo")]
        //public async Task<IActionResult> GetPremisInfo(int gid)
        //{
        //    try
        //    {
        //        premis_view result = new premis_view();

        //        result = await _dbContext.mst_premis.Where(x => x.gid == gid).Select(x => new premis_view
        //        {
        //            gid = x.gid,
        //            lot = x.lot,
        //            status_cukai = x.tempoh_sah_cukai == null
        //                        ? "None"
        //                        : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",
        //            status_lesen = x.tempoh_sah_lesen == null
        //                        ? "None"
        //                        : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",
        //            no_cukai = GenerateRandomString(10),
        //            no_lesen = GenerateRandomString(8),
        //            nama_perniagaan = GenerateRandomString(15),
        //            nama_pemilik = GenerateRandomString(12),
        //            alamat_premis1 = GenerateRandomString(30),
        //            alamat_premis2 = GenerateRandomString(30),
        //            status_notice = random.Next(0, 2) == 0 ? "Issued" : "Not Issued",
        //            status_expired_notice = random.Next(0, 2) == 0 ? "Yes" : "No",
        //            status_kompaun = random.Next(0, 2) == 0 ? "Paid" : "Unpaid",
        //            status_nota_pemeriksaan = random.Next(0, 2) == 0 ? "Passed" : "Failed"
        //        }).AsNoTracking().FirstOrDefaultAsync();

        //        result.gambar_premis = new List<string>
        //        {
        //            "https://theedgemalaysia.com/_next/image?url=https%3A%2F%2Fassets.theedgemarkets.com%2FPos%20Malaysia%20opens%20Pos%20Shop%2C%20its%20first%20convenience%20shop_20230508140716_pos%20malaysia.jpg&w=1920&q=75",
        //            "https://static.instaweb.my/uploads/wysiwyg/images/104707401665077/kedai-pc.png",
        //            "https://scontent-sin2-1.xx.fbcdn.net/v/t39.30808-6/301583885_428763489351387_8158376777441457793_n.jpg?stp=dst-jpg_s960x960_tt6&_nc_cat=102&ccb=1-7&_nc_sid=cc71e4&_nc_ohc=AfSXStBry2gQ7kNvgG8WM7T&_nc_zt=23&_nc_ht=scontent-sin2-1.xx&_nc_gid=Az8uOLVFqJqFGT_W_I6yY4e&oh=00_AYAZ2a7uqCgVkoNpoGgqJZYaX_z02VlYTGx1_iy9647Crw&oe=674DD89E",
        //            "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEhsQfRR0OfAPz-CmrgdmrIYLlme02bU1_ZLRiocjBs0HHNiGTlCXCtUf6e5YpTrGXaxHeO4V7pWJNqoBz4aWnHyKuQZTYelsv0MRxdNH8FqxAg3cDh_SHlFrkb8jtJdluK5Cz_UnxbQIY8/s1600/IMG_3094jk.jpg",
        //            "https://malaysiapropertysearch.net/wp-content/uploads/2019/09/288951-500x281.jpg",
        //        };

        //        result.lesen = new List<premis_license_view>();
        //        result.lesen.Add(new premis_license_view
        //        {
        //            aras = "G",
        //            status_lesen = result.status_lesen,
        //            no_lesen = GenerateRandomString(8),
        //            nama_perniagaan = GenerateRandomString(15),
        //            nama_pemilik = GenerateRandomString(12),
        //            alamat_premis = GenerateRandomString(30)
        //        });

        //        result.lesen.Add(new premis_license_view
        //        {
        //            aras = "1",
        //            status_lesen = result.status_lesen,
        //            no_lesen = GenerateRandomString(8),
        //            nama_perniagaan = GenerateRandomString(15),
        //            nama_pemilik = GenerateRandomString(12),
        //            alamat_premis = GenerateRandomString(30)
        //        });

        //        result.lesen.Add(new premis_license_view
        //        {
        //            aras = "2",
        //            status_lesen = result.status_lesen,
        //            no_lesen = GenerateRandomString(8),
        //            nama_perniagaan = GenerateRandomString(15),
        //            nama_pemilik = GenerateRandomString(12),
        //            alamat_premis = GenerateRandomString(30)
        //        });

        //        if (result == null)
        //        {
        //            return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
        //        }

        //        return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        //[HttpGet]
        //[Route("GetHistoryList")]
        //public async Task<IActionResult> GetHistoryList(int? crs = null)
        //{
        //    try
        //    {

        //        IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

        //        if (crs != null && crs == _defCRS)
        //        {
        //            initQuery = initQuery
        //                .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
        //        }

        //        var mst_premis = await initQuery
        //        .Select(x => new PremisMarkerViewModel
        //        {
        //            gid = x.gid,
        //            lot = x.lot,
        //            status_cukai = x.tempoh_sah_cukai == null
        //                        ? "None"
        //                        : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",
        //            status_lesen = x.tempoh_sah_lesen == null
        //                        ? "None"
        //                        : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                            ? "Active"
        //                            : "Expired",

        //            geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
        //        })
        //        .ToListAsync();


        //        if (mst_premis.Count == 0)
        //        {
        //            return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
        //        }

        //        //mst_lots = await _dbContext.mst_lots.AsNoTracking().ToListAsync();
        //        return Ok(mst_premis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        //[HttpGet]
        //[Route("GetFilteredListByBound")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetFilteredListByBound(double minLng, double minLat, double maxLng, double maxLat, string? filterType, int? crs = null)
        //{
        //    try
        //    {
        //        // Retrieve initial list of mst_premis
        //        IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

        //        if (crs == null || crs == _defCRS)
        //        {
        //            crs = _defCRS;
        //            initQuery = initQuery
        //                .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
        //        }
        //        else
        //        {
        //            initQuery = initQuery
        //                .Where(x => PostGISFunctions.ST_Within(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
        //                .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
        //        }

        //        var mst_premisList = await initQuery
        //            .Select(x => new PremisMarkerViewModel
        //            {
        //                gid = x.gid,
        //                lot = x.lot,
        //                status_cukai = x.tempoh_sah_cukai == null
        //                    ? "Tiada Data"
        //                    : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
        //                        ? "Dibayar"
        //                        : "Tertunggak",
        //                status_lesen = x.tempoh_sah_lesen == null
        //                    ? "Tiada Data"
        //                    : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                        ? "Aktif"
        //                        : "Tamat Tempoh",
        //                geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
        //            })
        //            .ToListAsync();

        //        // Check if any records were found
        //        if (!mst_premisList.Any())
        //        {
        //            return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
        //        }

        //        // For each mst_premis, get premis_view and add licenses
        //        var resultData = new List<premis_marker>();
        //        var gids = mst_premisList.Select(p => p.gid).ToList();

        //        // Fetch all premis_view data for the retrieved gids in one query
        //        var premisViews = await _dbContext.mst_premis
        //            .Where(x => gids.Contains(x.gid))
        //            .Select(x => new premis_view
        //            {
        //                gid = x.gid,
        //                // Include other necessary fields here
        //                status_lesen = x.tempoh_sah_lesen == null
        //                    ? "Tiada Data"
        //                    : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
        //                        ? "Aktif"
        //                        : "Tamat Tempoh",
        //            })
        //            .AsNoTracking()
        //            .ToListAsync();

        //        // Populate lesen information for each premis_view
        //        foreach (var premisView in premisViews)
        //        {
        //            premisView.lesen = new List<premis_license_view>
        //            {
        //                new premis_license_view { aras = "G", status_lesen = premisView.status_lesen, no_lesen = GenerateRandomString(8), nama_perniagaan = GenerateRandomString(15), nama_pemilik = GenerateRandomString(12), alamat_premis = GenerateRandomString(30) },
        //                new premis_license_view { aras = "1", status_lesen = premisView.status_lesen, no_lesen = GenerateRandomString(8), nama_perniagaan = GenerateRandomString(15), nama_pemilik = GenerateRandomString(12), alamat_premis = GenerateRandomString(30) },
        //                new premis_license_view { aras = "2", status_lesen = premisView.status_lesen, no_lesen = GenerateRandomString(8), nama_perniagaan = GenerateRandomString(15), nama_pemilik = GenerateRandomString(12), alamat_premis = GenerateRandomString(30) }
        //            };
        //        }

        //        // Build the result object for each mst_premis and include the corresponding lesen information
        //        foreach (var premis in mst_premisList)
        //        {
        //            // Find the corresponding premisView based on gid
        //            var correspondingPremisView = premisViews.FirstOrDefault(pv => pv.gid == premis.gid);

        //            // filter start here
        //            // Extract status_lesen values into a list from lesen and add premis.status_lesen
        //            var statusLesens = new List<string> { premis.status_lesen }; // Start with the status_lesen from mst_premis

        //            if (correspondingPremisView?.lesen != null)
        //            {
        //                statusLesens.AddRange(correspondingPremisView.lesen.Select(l => l.status_lesen));
        //            }

        //            // Split the filterType string into a list of statuses
        //            List<string> statusFilters;

        //            if (!string.IsNullOrEmpty(filterType))
        //            {
        //                // Split the filterType string into a list of statuses and trim whitespace
        //                statusFilters = filterType.Split(',')
        //                                          .Select(f => f.Trim())
        //                                          .ToList();
        //            }
        //            else
        //            {
        //                // Assign an empty list if filterType is null or empty
        //                statusFilters = new List<string>();
        //            }

        //            // Check if any of the status_lesen contains any of the values from filterType
        //            bool containsSearchTerm = false;
        //            bool isAktif = false;
        //            bool isTamatTempoh = false;
        //            bool isGantung = false;
        //            bool isTiadaData = false;
        //            bool isDibayar = false;
        //            bool isTertunggak = false;

        //            if (statusFilters.Any())
        //            {
        //                containsSearchTerm = statusLesens.Any(sl => statusFilters.Any(sf => sl.Contains(sf, StringComparison.OrdinalIgnoreCase)));

        //                isAktif = statusFilters.Contains("Aktif", StringComparer.OrdinalIgnoreCase) &&
        //                    statusLesens.Any(sl => sl.Equals("Aktif", StringComparison.OrdinalIgnoreCase));

        //                isTamatTempoh = statusFilters.Contains("Tamat Tempoh", StringComparer.OrdinalIgnoreCase) &&
        //                    statusLesens.Any(sl => sl.Equals("Tamat Tempoh", StringComparison.OrdinalIgnoreCase));

        //                isGantung = statusFilters.Contains("Gantung", StringComparer.OrdinalIgnoreCase) &&
        //                    statusLesens.Any(sl => sl.Equals("Gantung", StringComparison.OrdinalIgnoreCase));

        //                isTiadaData = statusFilters.Contains("Tiada Data", StringComparer.OrdinalIgnoreCase) &&
        //                    statusLesens.Any(sl => sl.Equals("Tiada Data", StringComparison.OrdinalIgnoreCase));

        //                isDibayar = statusFilters.Contains("Dibayar", StringComparer.OrdinalIgnoreCase) &&
        //                    premis.status_cukai.IndexOf("Dibayar", StringComparison.OrdinalIgnoreCase) >= 0;

        //                isTertunggak = statusFilters.Contains("Tertunggak", StringComparer.OrdinalIgnoreCase) &&
        //                    premis.status_cukai.IndexOf("Tertunggak", StringComparison.OrdinalIgnoreCase) >= 0;
        //            }

        //            String marker_cukai_status = "Default";
        //            String marker_lesen_status = "Default";
        //            String marker_color = "Black";

        //            // determine marker cukai status
        //            if (isTertunggak)
        //            {
        //                marker_cukai_status = "Tertunggak";
        //            }
        //            else if (isDibayar)
        //            {
        //                marker_cukai_status = "Dibayar";
        //            }

        //            // determine marker lesen status
        //            if (isTamatTempoh)
        //            {
        //                marker_lesen_status = "Tamat Tempoh";
        //            }
        //            else if (isGantung)
        //            {
        //                marker_lesen_status = "Gantung";
        //            }
        //            else if (isAktif)
        //            {
        //                marker_lesen_status = "Aktif";
        //            }
        //            else if (isTiadaData)
        //            {
        //                marker_lesen_status = "Tiada Data";
        //            }

        //            // determine marker color
        //            if (isTertunggak || isTamatTempoh)
        //            {
        //                marker_color = "Red";
        //            }
        //            else if (isDibayar || isAktif)
        //            {
        //                marker_color = "Green";
        //            }
        //            else if (isGantung)
        //            {
        //                marker_color = "Yellow";
        //            }
        //            else if (isTiadaData)
        //            {
        //                marker_color = "Grey";
        //            }

        //            // filter end here

        //            // Build the result object for this premis (can uncommment the commented var if want to debug)
        //            resultData.Add(new premis_marker
        //            {
        //                gid = premis.gid,
        //                lot = premis.lot,
        //                //containsSearchTerm = containsSearchTerm,
        //                //isAktif = isAktif,
        //                //isTamatTempoh = isTamatTempoh,
        //                //isGantung = isGantung,
        //                //isTiadaData = isTiadaData,
        //                //isDibayar = isDibayar,
        //                //isTertunggak = isTertunggak,
        //                //status_cukai = premis.status_cukai,
        //                //status_lesen = premis.status_lesen,
        //                marker_cukai_status = marker_cukai_status,
        //                marker_lesen_status = marker_lesen_status,
        //                marker_color = marker_color,
        //                geom = premis.geom,
        //                //lesen = correspondingPremisView?.lesen // Safely access lesen if correspondingPremisView is found
        //            });
        //        }

        //        return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        //#region private logic
        //private static string GenerateRandomString(int length)
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    return new string(Enumerable.Range(0, length)
        //        .Select(_ => chars[random.Next(chars.Length)])
        //        .ToArray());
        //}
        //private static string GetRandomLicenseStatus()
        //{
        //    string[] statuses = { "Aktif", "Tamat Tempoh", "Gantung", "Tiada Data" };
        //    Random random = new Random();
        //    return statuses[random.Next(statuses.Length)];
        //}
        //#endregion

        //[HttpGet]
        //[Route("GetListByTabType")]
        //public async Task<IActionResult> GetListByTabType(string tabType)
        //{
        //    try
        //    {
        //        int runUserID = await getDefRunUserId();
        //        string runUser = await getDefRunUser();

        //        var result = from premis in _tenantDBContext.mst_premis

        //                           select new premis_history_view
        //                           {
        //                               gid = premis.id,
        //                               //no_lesen_premis = premis.lesen,
        //                               //tempoh_sah_lesen = premis.tempoh_sah_lesen,
        //                               //tempoh_sah_cukai = premis.tempoh_sah_cukai,
        //                               //lot = premis.lot,
        //                           };
        //        return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

        #endregion
    }
}
