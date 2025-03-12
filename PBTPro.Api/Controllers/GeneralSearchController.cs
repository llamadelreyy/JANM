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
                    initQuery = from mlpt in _tenantDBContext.mst_license_premis_taxes
                            join ml in _tenantDBContext.mst_licensees on mlpt.license_accno equals ml.license_accno into mlJoin
                            from ml in mlJoin.DefaultIfEmpty()
                            join mt in _tenantDBContext.mst_taxholders on mlpt.tax_accno equals mt.tax_accno into mtJoin
                            from mt in mtJoin.DefaultIfEmpty()
                            join mol in _tenantDBContext.mst_owner_licensees on ml.owner_icno equals mol.owner_icno into molJoin
                            from mol in molJoin.DefaultIfEmpty()
                            join mot in _tenantDBContext.mst_owner_premis on mt.owner_icno equals mot.owner_icno into motJoin
                            from mot in motJoin.DefaultIfEmpty()
                            where EF.Functions.Like(ml.license_accno.ToLower(), $"%{keyword.ToLower()}%")
                            || EF.Functions.Like(mt.tax_accno.ToLower(), $"%{keyword.ToLower()}%")
                            || EF.Functions.Like(ml.ssm_no.ToLower(), $"%{keyword.ToLower()}%")
                            || (ml != null && EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%"))
                            || (ml != null && EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mol != null && EF.Functions.Like(mol.owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mol != null && EF.Functions.Like(mol.owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mot != null && EF.Functions.Like(mot.owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                            || (mot != null && EF.Functions.Like(mot.owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
                            select new
                            {
                                ml.license_accno,
                                ml.ssm_no,
                                mt.tax_accno,
                                ml.business_name,
                                ml.business_addr,
                                license_owner_name = mol.owner_name,
                                license_owner_icno = mol.owner_icno,
                                premis_owner_name = mot.owner_name,
                                premis_owner_icno = mot.owner_icno,
                                licenseScore = EF.Functions.Like(ml.license_accno.ToLower(), $"%{keyword.ToLower()}%") ? 1 : 0,
                                taxScore = EF.Functions.Like(mt.tax_accno.ToLower(), $"%{keyword.ToLower()}%") ? 1 : 0,
                                ssmScore = EF.Functions.Like(ml.ssm_no.ToLower(), $"%{keyword.ToLower()}%") ? 1 : 0,
                                businessNameScore = (ml != null && EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                businessAddrScore = (ml != null && EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                licenseOwnerNameScore = (mol != null && EF.Functions.Like(mol.owner_name.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                licenseOwnerIcnoScore = (mol != null && EF.Functions.Like(mol.owner_icno.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                premisOwnerNameScore = (mol != null && EF.Functions.Like(mot.owner_name.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0,
                                PremisownerIcnoScore = (mol != null && EF.Functions.Like(mot.owner_icno.ToLower(), $"%{keyword.ToLower()}%")) ? 1 : 0
                            };
                }
                else
                {
                    initQuery = type.ToUpper() switch
                    {
                        "SSM" => from mp in _tenantDBContext.mst_licensees
                                    where EF.Functions.Like(mp.ssm_no.ToLower(), $"%{keyword.ToLower()}%")
                                    select new
                                    {
                                        mp.ssm_no,
                                        licenseScore = 0,
                                        taxScore = 0,
                                        ssmScore = 1,
                                        businessNameScore = 0,
                                        businessAddrScore = 0,
                                        licenseOwnerNameScore = 0,
                                        licenseOwnerIcnoScore = 0,
                                        premisOwnerNameScore = 0,
                                        PremisownerIcnoScore = 0
                                    },
                        "TAX" => from mp in _tenantDBContext.mst_taxholders
                                    where EF.Functions.Like(mp.tax_accno.ToLower(), $"%{keyword.ToLower()}%")
                                    select new
                                    {
                                        mp.tax_accno,
                                        licenseScore = 0,
                                        taxScore = 1,
                                        ssmScore = 0,
                                        businessNameScore = 0,
                                        businessAddrScore = 0,
                                        licenseOwnerNameScore = 0,
                                        licenseOwnerIcnoScore = 0,
                                        premisOwnerNameScore = 0,
                                        PremisownerIcnoScore = 0
                                    },
                        "LICENSE" => from mp in _tenantDBContext.mst_licensees
                                        where EF.Functions.Like(mp.license_accno.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            mp.license_accno,
                                            licenseScore = 1,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 0,
                                            businessAddrScore = 0,
                                            licenseOwnerNameScore = 0,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 0,
                                            PremisownerIcnoScore = 0
                                        },
                        "COMPANY" => from ml in _tenantDBContext.mst_licensees
                                        where EF.Functions.Like(ml.business_name.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            ml.business_name,
                                            licenseScore = 0,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 1,
                                            businessAddrScore = 0,
                                            licenseOwnerNameScore = 0,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 0,
                                            PremisownerIcnoScore = 0
                                        },
                        "ADDRESS" => from ml in _tenantDBContext.mst_licensees
                                        where EF.Functions.Like(ml.business_addr.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            ml.business_addr,
                                            licenseScore = 0,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 0,
                                            businessAddrScore = 1,
                                            licenseOwnerNameScore = 0,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 0,
                                            PremisownerIcnoScore = 0
                                        },
                        //"NRIC" => from ml in _tenantDBContext.mst_licensees
                        //            where EF.Functions.Like(ml.owner_icno.ToLower(), $"%{keyword.ToLower()}%")
                        //            select new
                        //            {
                        //                ml.owner_icno,
                        //                licenseScore = 0,
                        //                taxScore = 0,
                        //                ssmScore = 0,
                        //                businessNameScore = 0,
                        //                businessAddrScore = 0,
                        //                licenseOwnerNameScore = 0,
                        //                licenseOwnerIcnoScore = 1,
                        //                premisOwnerNameScore = 0,
                        //                PremisownerIcnoScore = 0
                        //            },
                        "NRIC" => (from ml in _tenantDBContext.mst_licensees
                                    where EF.Functions.Like(ml.owner_icno.ToLower(), $"%{keyword.ToLower()}%")
                                    select new
                                    {
                                        license_owner_icno = ml.owner_icno,
                                        premis_owner_icno = "",
                                        licenseScore = 0,
                                        taxScore = 0,
                                        ssmScore = 0,
                                        businessNameScore = 0,
                                        businessAddrScore = 0,
                                        licenseOwnerNameScore = 0,
                                        licenseOwnerIcnoScore = 1,
                                        premisOwnerNameScore = 0,
                                        PremisownerIcnoScore = 0
                                    })
                                    .Union(
                                        from mt in _tenantDBContext.mst_taxholders
                                        where EF.Functions.Like(mt.owner_icno.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            license_owner_icno = "",
                                            premis_owner_icno = mt.tax_accno,
                                            licenseScore = 0,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 0,
                                            businessAddrScore = 0,
                                            licenseOwnerNameScore = 0,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 0,
                                            PremisownerIcnoScore = 1
                                        }),
                        //"OWNER" => from mo in _tenantDBContext.mst_owner_licensees
                        //           where EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%")
                        //           select new
                        //           {
                        //               mo.owner_name,
                        //               licenseScore = 0,
                        //               taxScore = 0,
                        //               businessNameScore = 0,
                        //               businessAddrScore = 0,
                        //               licenseOwnerNameScore = 1,
                        //               licenseOwnerIcnoScore = 0,
                        //               premisOwnerNameScore = 0,
                        //               PremisownerIcnoScore = 0
                        //           },
                        "OWNER" => (from mo in _tenantDBContext.mst_owner_licensees
                                        where EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            license_owner_name = mo.owner_name,
                                            premis_owner_name = "",
                                            licenseScore = 0,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 0,
                                            businessAddrScore = 0,
                                            licenseOwnerNameScore = 1,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 0,
                                            PremisownerIcnoScore = 0
                                        })
                                    .Union(
                                        from mo in _tenantDBContext.mst_owner_premis
                                        where EF.Functions.Like(mo.owner_name.ToLower(), $"%{keyword.ToLower()}%")
                                        select new
                                        {
                                            license_owner_name = "",
                                            premis_owner_name = mo.owner_name,
                                            licenseScore = 0,
                                            taxScore = 0,
                                            ssmScore = 0,
                                            businessNameScore = 0,
                                            businessAddrScore = 0,
                                            licenseOwnerNameScore = 0,
                                            licenseOwnerIcnoScore = 0,
                                            premisOwnerNameScore = 1,
                                            PremisownerIcnoScore = 0
                                        }),
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

                //resultsWithHighestMatch = results.Select(result =>
                //{
                //    string highestMatch = result.licenseScore > result.taxScore && result.licenseScore > result.businessNameScore && result.licenseScore > result.businessAddrScore && result.licenseScore > result.ownerNameScore && result.licenseScore > result.ownerIcnoScore
                //        ? result.lesen
                //        : result.taxScore > result.businessNameScore && result.taxScore > result.businessAddrScore && result.taxScore > result.ownerNameScore && result.taxScore > result.ownerIcnoScore
                //        ? result.no_akaun
                //        : result.businessNameScore > result.businessAddrScore && result.businessNameScore > result.ownerNameScore && result.businessNameScore > result.ownerIcnoScore
                //        ? result.business_name
                //        : result.businessAddrScore > result.ownerNameScore && result.businessAddrScore > result.ownerIcnoScore
                //        ? result.business_addr
                //        : result.ownerNameScore > result.ownerIcnoScore
                //        ? result.owner_name
                //        : result.owner_icno;

                //    return highestMatch;
                //}).ToList();

                resultsWithHighestMatch = results.Select(result =>
                {
                    int highestMatchScore = new[]
                    {
                        result.taxScore,
                        result.ssmScore,
                        result.businessNameScore,
                        result.businessAddrScore,
                        result.licenseOwnerNameScore,
                        result.licenseOwnerIcnoScore,
                        result.premisOwnerNameScore,
                        result.PremisownerIcnoScore
                    }.Max();

                    string highestMatch = highestMatchScore == result.taxScore ? result.tax_accno :
                                            highestMatchScore == result.ssmScore ? result.ssm_no :
                                            highestMatchScore == result.businessNameScore ? result.business_name :
                                            highestMatchScore == result.businessAddrScore ? result.business_addr :
                                            highestMatchScore == result.licenseOwnerNameScore ? result.license_owner_name :
                                            highestMatchScore == result.licenseOwnerIcnoScore ? result.license_owner_icno :
                                            highestMatchScore == result.premisOwnerNameScore ? result.premis_owner_name :
                                            result.premis_owner_icno;

                    return new
                    {
                        result,
                        highestMatchScore,
                        highestMatch
                    };
                })
                .OrderByDescending(x => x.highestMatchScore) 
                .Select(x => x.highestMatch) 
                .ToList();

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
                var initQuery = from mlpt in _tenantDBContext.mst_license_premis_taxes
                                join mp in _tenantDBContext.mst_premis on mlpt.codeid_premis equals mp.codeid_premis into mpJoin
                                from mp in mpJoin.DefaultIfEmpty()
                                join ml in _tenantDBContext.mst_licensees on mlpt.license_accno equals ml.license_accno into mlJoin
                                from ml in mlJoin.DefaultIfEmpty()
                                join mt in _tenantDBContext.mst_taxholders on mlpt.tax_accno equals mt.tax_accno into mtJoin
                                from mt in mtJoin.DefaultIfEmpty()
                                join mol in _tenantDBContext.mst_owner_licensees on ml.owner_icno equals mol.owner_icno into molJoin
                                from mol in molJoin.DefaultIfEmpty()
                                join mot in _tenantDBContext.mst_owner_premis on mt.owner_icno equals mot.owner_icno into motJoin
                                from mot in motJoin.DefaultIfEmpty()
                                join rls in _tenantDBContext.ref_license_statuses on ml.status_id equals rls.status_id into rlsJoin
                                from rls in rlsJoin.DefaultIfEmpty()
                                join rts in _tenantDBContext.ref_tax_statuses on mt.status_id equals rts.status_id into rtsJoin
                                from rts in rtsJoin.DefaultIfEmpty()
                                select new
                                {
                                    mp.codeid_premis,
                                    mp.geom,
                                    mt.tax_accno,
                                    ml.license_accno,
                                    ml.ssm_no,
                                    ml.business_name,
                                    ml.business_addr,
                                    ml.status_id,
                                    license_owner_name = mol.owner_name,
                                    license_owner_icno = mol.owner_icno,
                                    premis_owner_name = mot.owner_name,
                                    premis_owner_icno = mot.owner_icno,
                                    license_status = rls.status_name,
                                    tax_status = rts.status_name,
                                };

                initQuery = (type?.ToUpper() ?? string.Empty) switch
                {
                    "SSM" => initQuery.Where(x => EF.Functions.Like(x.ssm_no.ToLower(), $"%{keyword.ToLower()}%")),
                    "TAX" => initQuery.Where(x => EF.Functions.Like(x.tax_accno.ToLower(), $"%{keyword.ToLower()}%")),
                    "LICENSE" => initQuery.Where(x => EF.Functions.Like(x.license_accno.ToLower(), $"%{keyword.ToLower()}%")),
                    "COMPANY" => initQuery.Where(x => EF.Functions.Like(x.business_name.ToLower(), $"%{keyword.ToLower()}%")),
                    "ADDRESS" => initQuery.Where(x => EF.Functions.Like(x.business_addr.ToLower(), $"%{keyword.ToLower()}%")),
                    "NRIC" => initQuery.Where(x => EF.Functions.Like(x.license_owner_icno.ToLower(), $"%{keyword.ToLower()}%") || EF.Functions.Like(x.premis_owner_icno.ToLower(), $"%{keyword.ToLower()}%")),
                    "OWNER" => initQuery.Where(x => EF.Functions.Like(x.license_owner_name.ToLower(), $"%{keyword.ToLower()}%") || EF.Functions.Like(x.premis_owner_name.ToLower(), $"%{keyword.ToLower()}%")),
                    _ => initQuery = initQuery.Where(x => EF.Functions.Like(x.license_accno.ToLower(), $"%{keyword.ToLower()}%")
                        || EF.Functions.Like(x.tax_accno.ToLower(), $"%{keyword.ToLower()}%")
                        || EF.Functions.Like(x.ssm_no.ToLower(), $"%{keyword.ToLower()}%")
                        || (x.business_name != null && EF.Functions.Like(x.business_name.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.business_addr != null && EF.Functions.Like(x.business_addr.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.license_owner_name != null && EF.Functions.Like(x.license_owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.license_owner_icno != null && EF.Functions.Like(x.license_owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.premis_owner_name != null && EF.Functions.Like(x.premis_owner_name.ToLower(), $"%{keyword.ToLower()}%"))
                        || (x.premis_owner_icno != null && EF.Functions.Like(x.premis_owner_icno.ToLower(), $"%{keyword.ToLower()}%"))
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
                        x.codeid_premis,
                        geom = PostGISFunctions.ParseGeoJsonSafely(PostGISFunctions.ST_AsGeoJSON(x.geom)),
                        x.business_name,
                        x.business_addr,
                        x.status_id,
                        x.license_accno,
                        x.license_owner_name,
                        x.license_owner_icno,
                        x.license_status,
                        x.tax_accno,
                        x.premis_owner_name,
                        x.premis_owner_icno,
                        x.tax_status
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
