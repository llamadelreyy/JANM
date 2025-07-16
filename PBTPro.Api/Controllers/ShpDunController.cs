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
    public class ShpDunController : IBaseController
    {
        private readonly ILogger<ShpDunController> _logger;
        private readonly string _feature = "SHP_DUN";
        private readonly int _defCRS = 4326;

        public ShpDunController(PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<ShpDunController> logger) : base(dbContext)
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
                IQueryable<shp_dun> initQuery = _tenantDBContext.shp_duns.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs != null && crs == _defCRS)
                {
                    initQuery = initQuery
                        .Select(x => new shp_dun { id = x.id, Name = x.Name, geom = (NetTopologySuite.Geometries.MultiPolygon)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var mst_lots = await initQuery
                .Select(x => new DunPolygonViewModel
                {
                    gid = x.id,
                    name = x.Name,
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
                IQueryable<shp_dun> initQuery = _tenantDBContext.shp_duns.Where(x => PostGISFunctions.ST_IsValid(x.geom));


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
                        .Select(x => new shp_dun { id = x.id, Name = x.Name, geom = (NetTopologySuite.Geometries.MultiPolygon)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var mst_lots = await initQuery
                .Select(x => new DunPolygonViewModel
                {
                    gid = x.id,
                    name = x.Name,
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

        #endregion
    }
}
