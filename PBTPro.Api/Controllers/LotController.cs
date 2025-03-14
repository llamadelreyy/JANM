/*
Project: PBT Pro
Description: sample for lot polygon point 
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
05/11/2024 - initial create
24/11/2024 - change all hardcoded sql query to EF function
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Reflection;
using System.Text;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotController : IBaseController
    {
        private readonly ILogger<LotController> _logger;
        private readonly string _feature = "LOT";
        private readonly int _defCRS = 4326;

        public LotController(PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<LotController> logger) : base(dbContext)
        {
            _logger = logger;
            _tenantDBContext = tntdbContext;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList(int? crs = null)
        {
            try
            {
                IQueryable<mst_lot> initQuery = _tenantDBContext.mst_lots.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs != null && crs == _defCRS)
                {
                    initQuery = initQuery
                        .Select(x => new mst_lot { gid = x.gid, objectid = x.objectid, lot = x.lot, geom = (NetTopologySuite.Geometries.MultiPolygon)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var mst_lots = await initQuery
                .Select(x => new LotPolygonViewModel
                {
                    gid = x.gid,
                    objectid = x.objectid,
                    lot = x.lot,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
                })
                .ToListAsync();

                if (mst_lots.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                //mst_lots = await _tenantDBContext.mst_lots.AsNoTracking().ToListAsync();
                return Ok(mst_lots, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetListByBound")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, int? crs = null)
        {
            try
            {
                IQueryable<mst_lot> initQuery = _tenantDBContext.mst_lots.Where(x => PostGISFunctions.ST_IsValid(x.geom));


                if (crs == null || crs == _defCRS)
                {
                    crs = _defCRS;
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Intersects(x.geom, PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value)));
                }
                else
                {
                    initQuery = initQuery
                        .Where(x => PostGISFunctions.ST_Intersects(x.geom, PostGISFunctions.ST_Transform(PostGISFunctions.ST_MakeEnvelope(minLng, minLat, maxLng, maxLat, crs.Value), _defCRS)))
                        .Select(x => new mst_lot { gid = x.gid, objectid = x.objectid, lot = x.lot, geom = (NetTopologySuite.Geometries.MultiPolygon)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var mst_lots = await initQuery
                .Select(x => new LotPolygonViewModel
                {
                    gid = x.gid,
                    objectid = x.objectid,
                    lot = x.lot,
                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
                })
                .ToListAsync();

                if (mst_lots.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                //mst_lots = await _tenantDBContext.mst_lots.AsNoTracking().ToListAsync();
                return Ok(mst_lots, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region private logic
        /*
        private string BuildGeomSelect(bool transformGeom = false, string? crs = null)
        {

            if (string.IsNullOrWhiteSpace(crs))
            {
                crs = _defCRS;
            }

            var selectBuilder = new StringBuilder("SELECT gid, lot, ");

            if (transformGeom)
            {
                selectBuilder.Append($"ST_Transform(geom, {crs}) AS geom, ");
            }
            else
            {
                selectBuilder.Append($"geom, ");
            }

            selectBuilder.Length -= 2;
            selectBuilder.Append(" FROM mst_lot");

            return selectBuilder.ToString();
        }
        
        private string BuildDynamicSelect<TEntity>(bool transformGeom = false, string? crs = null)
        {

            if (string.IsNullOrWhiteSpace(crs))
            {
                crs = _defCRS;
            }

            var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var selectBuilder = new StringBuilder("SELECT ");

            foreach (var property in properties)
            {
                string columnName = property.Name;

                if (property.Name == "geom")
                {
                    if (transformGeom)
                    {
                        selectBuilder.Append($"ST_Transform(geom, {crs}) AS geom, ");
                    }
                    else
                    {
                        selectBuilder.Append($"geom AS geom, ");
                    }
                }
                else
                {
                    columnName = ConvertToSnakeCase(property.Name);
                    selectBuilder.Append($"{columnName}, ");
                }
            }

            selectBuilder.Length -= 2;
            selectBuilder.Append(" FROM mst_lot");

            return selectBuilder.ToString();
        }

        private string ConvertToSnakeCase(string propertyName)
        {
            return string.Concat(propertyName.Select((c, i) =>
                i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();
        }
        */
        #endregion
    }
}
