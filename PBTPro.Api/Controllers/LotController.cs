using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;
using System.Linq;
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
        private readonly string _defCRS = "3375";

        public LotController(PBTProDbContext dbContext, ILogger<LotController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList(string? crs = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(crs))
                {
                    crs = _defCRS;
                }
                //List<MstLot> mstLots = new List<MstLot>();
                var query = BuildDynamicSelect<MstLot>(transformGeom: true, crs: crs);
                //string query = $"SELECT Id, ST_Transform(geom, {crs}) AS geom, objectid, negeri, daerah, mukim, seksyen, lot, upi, s_area, m_area, g_area, unit, pa, refplan, apdate, cls, landusecod, landtitlec, entrymode, updated, guid, mi_prinx, shape_leng, shape_area FROM mst_lot";
                //string query = $"SELECT *, ST_Transform(geom, {crs}) AS geom_transformed FROM mst_lot";
                var mstLots = _dbContext.MstLots
                    .FromSqlRaw(query)
                    .ToList();

                if (mstLots.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                //mstLots = await _dbContext.MstLots.AsNoTracking().ToListAsync();
                return Ok(mstLots, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [Route("GetListByBound")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, string? crs = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(crs))
                {
                    crs = _defCRS;
                }
                string query = BuildDynamicSelect<MstLot>(transformGeom: true, crs: crs);
                query = $"{query} WHERE ST_Within(geom, ST_Transform(ST_MakeEnvelope({minLng}, {minLat}, {maxLng}, {maxLat}, {crs}),{_defCRS}))";
                var mstLots = _dbContext.MstLots
                    .FromSqlRaw(query)
                    .ToList();

                if (mstLots.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                //mstLots = await _dbContext.MstLots.AsNoTracking().ToListAsync();
                return Ok(mstLots, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Data lot berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }




        #region private logic
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

                if (property.Name == "Geom")
                {
                    selectBuilder.Append($"ST_Transform(geom, {crs}) AS geom, ");
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
        #endregion
    }
}
