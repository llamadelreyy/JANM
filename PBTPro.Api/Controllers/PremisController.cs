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
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Overlay;
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
    public class PremisController : IBaseController
    {
        private readonly ILogger<PremisController> _logger;
        private readonly string _feature = "PREMIS";
        private readonly int _defCRS = 4326;

        public PremisController(PBTProDbContext dbContext, ILogger<PremisController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList(int? crs = null)
        {
            try
            {

                IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));

                if (crs != null && crs == _defCRS)
                {
                    initQuery = initQuery
                        .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom, crs.Value) });
                }

                var mst_premis = await initQuery
                .Select(x => new PremisMarkerViewModel
                {
                    gid = x.gid,
                    lot = x.lot,
                    cukai_status = x.tempoh_sah_cukai == null
                                ? "tiada"
                                : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
                                    ? "hidup"
                                    : "mati",

                    lesen_status = x.tempoh_sah_lesen == null
                                ? "tiada"
                                : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
                                    ? "hidup"
                                    : "mati",

                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
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
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, int? crs = null)
        {
            try
            {
                IQueryable<mst_premis> initQuery = _dbContext.mst_premis.Where(x => PostGISFunctions.ST_IsValid(x.geom));


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
                        .Select(x => new mst_premis { gid = x.gid, geom = (NetTopologySuite.Geometries.Point)PostGISFunctions.ST_Transform(x.geom,crs.Value) });
                }

                var mst_premis = await initQuery
                .Select(x => new PremisMarkerViewModel
                {
                    gid = x.gid,
                    lot = x.lot,
                    cukai_status = x.tempoh_sah_cukai == null
                                ? "tiada"
                                : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
                                    ? "hidup"
                                    : "mati",

                                        lesen_status = x.tempoh_sah_lesen == null
                                ? "tiada"
                                : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
                                    ? "hidup"
                                    : "mati",

                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
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

        #region private logic
        #endregion
    }
}
