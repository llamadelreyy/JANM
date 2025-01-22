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
using System;
using System.Linq;
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
        private static Random random = new Random(); //just to create random dummy data, will need to remove this later

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
                    status_cukai = x.tempoh_sah_cukai == null
                                ? "None"
                                : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",
                    status_lesen = x.tempoh_sah_lesen == null
                                ? "None"
                                : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",

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
        public async Task<IActionResult> GetListByBound(double minLng, double minLat, double maxLng, double maxLat, int? crs = null, List<String> filterType = null)
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
                    status_cukai = x.tempoh_sah_cukai == null
                                ? "None"
                                : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",
                    status_lesen = x.tempoh_sah_lesen == null
                                ? "None"
                                : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",

                    geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
                })
                .ToListAsync();

                if (filterType != null && filterType.Any())
                {
                    mst_premis = (List<PremisMarkerViewModel>)initQuery.Where(x => filterType.Contains(Convert.ToString(x.tempoh_sah_cukai)));
                }

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
        [Route("GetPremisInfo")]
        public async Task<IActionResult> GetPremisInfo(int gid)
        {
            try
            {
                premis_view result = new premis_view();

                result = await _dbContext.mst_premis.Where(x => x.gid == gid).Select(x => new premis_view
                {
                    gid = x.gid,
                    lot = x.lot,
                    status_cukai = x.tempoh_sah_cukai == null
                                ? "None"
                                : x.tempoh_sah_cukai > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",
                    status_lesen = x.tempoh_sah_lesen == null
                                ? "None"
                                : x.tempoh_sah_lesen > DateOnly.FromDateTime(DateTime.Now)
                                    ? "Active"
                                    : "Expired",
                    no_cukai = GenerateRandomString(10),
                    no_lesen = GenerateRandomString(8),
                    nama_perniagaan = GenerateRandomString(15),
                    nama_pemilik = GenerateRandomString(12),  
                    alamat_premis1 = GenerateRandomString(30),
                    alamat_premis2 = GenerateRandomString(30),
                    status_notice = random.Next(0, 2) == 0 ? "Issued" : "Not Issued",
                    status_expired_notice = random.Next(0, 2) == 0 ? "Yes" : "No",
                    status_kompaun = random.Next(0, 2) == 0 ? "Paid" : "Unpaid",
                    status_nota_pemeriksaan = random.Next(0, 2) == 0 ? "Passed" : "Failed"
                }).AsNoTracking().FirstOrDefaultAsync();

                result.gambar_premis = new List<string>
                {
                    "https://theedgemalaysia.com/_next/image?url=https%3A%2F%2Fassets.theedgemarkets.com%2FPos%20Malaysia%20opens%20Pos%20Shop%2C%20its%20first%20convenience%20shop_20230508140716_pos%20malaysia.jpg&w=1920&q=75",
                    "https://static.instaweb.my/uploads/wysiwyg/images/104707401665077/kedai-pc.png",
                    "https://scontent-sin2-1.xx.fbcdn.net/v/t39.30808-6/301583885_428763489351387_8158376777441457793_n.jpg?stp=dst-jpg_s960x960_tt6&_nc_cat=102&ccb=1-7&_nc_sid=cc71e4&_nc_ohc=AfSXStBry2gQ7kNvgG8WM7T&_nc_zt=23&_nc_ht=scontent-sin2-1.xx&_nc_gid=Az8uOLVFqJqFGT_W_I6yY4e&oh=00_AYAZ2a7uqCgVkoNpoGgqJZYaX_z02VlYTGx1_iy9647Crw&oe=674DD89E",
                    "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEhsQfRR0OfAPz-CmrgdmrIYLlme02bU1_ZLRiocjBs0HHNiGTlCXCtUf6e5YpTrGXaxHeO4V7pWJNqoBz4aWnHyKuQZTYelsv0MRxdNH8FqxAg3cDh_SHlFrkb8jtJdluK5Cz_UnxbQIY8/s1600/IMG_3094jk.jpg",
                    "https://malaysiapropertysearch.net/wp-content/uploads/2019/09/288951-500x281.jpg",
                };

                result.lesen = new List<premis_license_view>();
                result.lesen.Add(new premis_license_view
                {
                    aras = "G",
                    status_lesen = result.status_lesen,
                    no_lesen = GenerateRandomString(8),
                    nama_perniagaan = GenerateRandomString(15),
                    nama_pemilik = GenerateRandomString(12),
                    alamat_premis = GenerateRandomString(30)
                });

                result.lesen.Add(new premis_license_view
                {
                    aras = "1",
                    status_lesen = result.status_lesen,
                    no_lesen = GenerateRandomString(8),
                    nama_perniagaan = GenerateRandomString(15),
                    nama_pemilik = GenerateRandomString(12),
                    alamat_premis = GenerateRandomString(30)
                });

                result.lesen.Add(new premis_license_view
                {
                    aras = "2",
                    status_lesen = result.status_lesen,
                    no_lesen = GenerateRandomString(8),
                    nama_perniagaan = GenerateRandomString(15),
                    nama_pemilik = GenerateRandomString(12),
                    alamat_premis = GenerateRandomString(30)
                });

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


        #region private logic
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
        #endregion
    }
}
