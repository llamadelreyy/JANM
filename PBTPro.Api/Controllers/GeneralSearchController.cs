/*
Project: PBT Pro
Description: General Search controller
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
11/02/2025 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class GeneralSearchController : IBaseController
    {
        private readonly ILogger<GeneralSearchController> _logger;
        private readonly string _feature = "GENERAL_SEARCH";

        public GeneralSearchController(PBTProDbContext dbContext, ILogger<GeneralSearchController> logger) : base(dbContext)
        {
            _logger = logger;
            SetTenantDbContext("tenant");
        }

        [HttpGet]
        [AllowAnonymous]
        //type = SSM, TAX, LICENSE, COMPANY, NRIC, ADDRESS, OWNER
        public async Task<IActionResult> GetSearchSuggestion(string keyword, string? type, int? maxResult = null, int? page = 1)
        {
            try
            {
                #region Building Query
                IQueryable<dynamic> initQuery = _tenantDBContext.mst_premis.AsQueryable();
                List<string> resultsWithHighestMatch = new List<string>();

                if (string.IsNullOrEmpty(type))
                {
                    initQuery = from mp in _tenantDBContext.mst_premis
                            join ml in _tenantDBContext.mst_licensees on mp.lesen equals ml.license_accno into mlJoin
                            from ml in mlJoin.DefaultIfEmpty()
                            join mo in _tenantDBContext.mst_owners on ml.owner_icno equals mo.owner_icno into moJoin
                            from mo in moJoin.DefaultIfEmpty()
                            where EF.Functions.Like(mp.lesen.ToLower(), $"%{keyword.ToLower()}%")
                            || EF.Functions.Like(mp.no_akaun.ToLower(), $"%{keyword.ToLower()}%")
                            || (ml != null && EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%"))
                            || (ml != null && EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mo != null && EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mo != null && EF.Functions.Like(mo.owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
                            select new
                            {
                                mp.lesen,
                                mp.no_akaun,
                                ml.business_name,
                                ml.business_addr,
                                mo.owner_name,
                                mo.owner_icno,
                                lesenscore = EF.Functions.Like(mp.lesen.ToLower(), $"%{keyword.ToLower()}%") ? 1 : 0,
                                noakauScore = EF.Functions.Like(mp.no_akaun.ToLower(), $"%{keyword.ToLower()}%") ? 1 : 0,
                                businessNameScore = (ml != null && EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                businessAddrScore = (ml != null && EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                ownerNameScore = (mo != null && EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                ownerIcnoScore = (mo != null && EF.Functions.Like(mo.owner_icno.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0
                            };
                }
                else
                {
                    initQuery = type.ToUpper() switch
                    {
                        "SSM" => from mp in _tenantDBContext.mst_premis
                                 where EF.Functions.Like(mp.lesen.ToLower(), $"%{keyword.ToLower()}%")
                                 select new
                                 {
                                     mp.lesen,
                                     lesenscore = 1,
                                     noakauScore = 0,
                                     businessNameScore = 0,
                                     businessAddrScore = 0,
                                     ownerNameScore = 0,
                                     ownerIcnoScore = 0
                                 },
                        "TAX" => from mp in _tenantDBContext.mst_premis
                                 where EF.Functions.Like(mp.no_akaun.ToLower(), $"%{keyword.ToLower()}%")
                                 select new
                                 {
                                     mp.no_akaun,
                                     lesenscore = 0,
                                     noakauScore = 1,
                                     businessNameScore = 0,
                                     businessAddrScore = 0,
                                     ownerNameScore = 0,
                                     ownerIcnoScore = 0
                                 },
                        "LICENSE" => from mp in _tenantDBContext.mst_premis
                                     where EF.Functions.Like(mp.lesen.ToLower(), $"%{keyword.ToLower()}%")
                                     select new
                                     {
                                         mp.lesen,
                                         lesenscore = 1,
                                         noakauScore = 0,
                                         businessNameScore = 0,
                                         businessAddrScore = 0,
                                         ownerNameScore = 0,
                                         ownerIcnoScore = 0
                                     },
                        "COMPANY" => from ml in _tenantDBContext.mst_licensees
                                     where EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%")
                                     select new
                                     {
                                         ml.business_name,
                                         lesenscore = 0,
                                         noakauScore = 0,
                                         businessNameScore = 1,
                                         businessAddrScore = 0,
                                         ownerNameScore = 0,
                                         ownerIcnoScore = 0
                                     },
                        "ADDRESS" => from ml in _tenantDBContext.mst_licensees
                                     where EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%")
                                     select new
                                     {
                                         ml.business_addr,
                                         lesenscore = 0,
                                         noakauScore = 0,
                                         businessNameScore = 0,
                                         businessAddrScore = 1,
                                         ownerNameScore = 0,
                                         ownerIcnoScore = 0
                                     },
                        "NRIC" => from ml in _tenantDBContext.mst_licensees
                                  where EF.Functions.Like(ml.owner_icno.ToLower(), $"%{keyword.ToLower()}%")
                                  select new
                                  {
                                      ml.owner_icno,
                                      lesenscore = 0,
                                      noakauScore = 0,
                                      businessNameScore = 0,
                                      businessAddrScore = 0,
                                      ownerNameScore = 0,
                                      ownerIcnoScore = 1
                                  },
                        "OWNER" => from mo in _tenantDBContext.mst_owners
                                   where EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%")
                                   select new
                                   {
                                       mo.owner_name,
                                       lesenscore = 0,
                                       noakauScore = 0,
                                       businessNameScore = 0,
                                       businessAddrScore = 0,
                                       ownerNameScore = 1,
                                       ownerIcnoScore = 0,
                                   },
                        _ => throw new ArgumentException("Invalid type selected")
                    };
                }

                initQuery = initQuery.Distinct();
                #endregion

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

                var results = await initQuery.ToListAsync();

                resultsWithHighestMatch = results.Select(result =>
                {
                    string highestMatch = result.lesenscore > result.noakauScore && result.lesenscore > result.businessNameScore && result.lesenscore > result.businessAddrScore && result.lesenscore > result.ownerNameScore && result.lesenscore > result.ownerIcnoScore
                        ? result.lesen
                        : result.noakauScore > result.businessNameScore && result.noakauScore > result.businessAddrScore && result.noakauScore > result.ownerNameScore && result.noakauScore > result.ownerIcnoScore
                        ? result.no_akaun
                        : result.businessNameScore > result.businessAddrScore && result.businessNameScore > result.ownerNameScore && result.businessNameScore > result.ownerIcnoScore
                        ? result.business_name
                        : result.businessAddrScore > result.ownerNameScore && result.businessAddrScore > result.ownerIcnoScore
                        ? result.business_addr
                        : result.ownerNameScore > result.ownerIcnoScore
                        ? result.owner_name
                        : result.owner_icno;

                    return highestMatch;
                }).ToList();

                if (resultsWithHighestMatch.Count == 0)
                {
                    resultsWithHighestMatch = new List<string>();
                }

                return Ok(resultsWithHighestMatch, pageInfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearchResult(string keyword, string? type, int? maxResult = null, int? page = 1)
        {
            try
            {
                #region Building Query
                var initQuery = from mp in _tenantDBContext.mst_premis
                            join ml in _tenantDBContext.mst_licensees on mp.lesen equals ml.license_accno into mlJoin
                            from ml in mlJoin.DefaultIfEmpty()
                            join mo in _tenantDBContext.mst_owners on ml.owner_icno equals mo.owner_icno into moJoin
                            from mo in moJoin.DefaultIfEmpty()
                            join rls in _tenantDBContext.ref_license_statuses on ml.status_id equals rls.status_id into rslJoin
                            from rls in rslJoin.DefaultIfEmpty()
                            select new
                            {
                                mp.gid,
                                mp.lesen,
                                mp.no_akaun,
                                mp.geom,
                                ml.business_name,
                                ml.business_addr,
                                ml.status_id,
                                mo.owner_name,
                                mo.owner_icno,
                                status_lesen = rls.status_name
                            };

                initQuery = (type?.ToUpper() ?? string.Empty) switch
                {
                    "SSM" => initQuery.Where(x => EF.Functions.Like(x.lesen.ToLower(), $"%{keyword.ToLower()}%")),
                    "TAX" => initQuery.Where(x => EF.Functions.Like(x.no_akaun.ToLower(), $"%{keyword.ToLower()}%")),
                    "LICENSE" => initQuery.Where(x => EF.Functions.Like(x.lesen.ToLower(), $"%{keyword.ToLower()}%")),
                    "COMPANY" => initQuery.Where(x => EF.Functions.Like(x.business_name.ToLower(), $"%{keyword.ToLower()}%")),
                    "ADDRESS" => initQuery.Where(x => EF.Functions.Like(x.business_addr.ToLower(), $"%{keyword.ToLower()}%")),
                    "NRIC" => initQuery.Where(x => EF.Functions.Like(x.owner_icno.ToLower(), $"%{keyword.ToLower()}%")),
                    "OWNER" => initQuery.Where(x => EF.Functions.Like(x.owner_name.ToLower(), $"%{keyword.ToLower()}%")),
                    _ => initQuery = initQuery.Where(x => EF.Functions.Like(x.lesen.ToLower(), $"%{keyword.ToLower()}%")
                        || EF.Functions.Like(x.no_akaun.ToLower(), $"%{keyword.ToLower()}%")
                        || (x.business_name != null && EF.Functions.Like(x.business_name.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.business_addr != null && EF.Functions.Like(x.business_addr.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.owner_name != null && EF.Functions.Like(x.owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.owner_icno != null && EF.Functions.Like(x.owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
                    )
                };
                initQuery = initQuery.Distinct();
                #endregion

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

                var results = await initQuery
                    .Select(x => new
                    {
                        x.gid,
                        x.lesen,
                        x.no_akaun,
                        geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
                        x.business_name,
                        x.business_addr,
                        x.status_id,
                        x.owner_name,
                        x.owner_icno,
                        x.status_lesen
                    })
                    .ToListAsync();

                return Ok(results, pageInfo, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        #region Private Logic
        #endregion
    }
}
