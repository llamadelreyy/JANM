/*
Project: PBT Pro
Description: License API controller to handle lesen 
Author: Nurulfarhana
Date: DEcember 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
*/

using AutoMapper.Internal;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using static Duende.IdentityServer.Models.IdentityResources;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LicenseController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LicenseController> _logger;

        private readonly string _feature = "LESEN";
        public LicenseController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<LicenseController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
            _tenantDBContext = tntdbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await (from license_hist in _dbContext.license_histories
                                          join license_info in _dbContext.license_informations
                                          on license_hist.hist_id_info equals license_info.license_id
                                          group new { license_hist, license_info } by license_hist.hist_id_info into g
                                          select new
                                          {
                                              Histories = g.Select(x => x.license_hist).ToList(),
                                              LicenseInfo = g.Select(x => x.license_info).ToList(),
                                              HistIdInfo = g.Key
                                          })
                                .FirstOrDefaultAsync(x => x.Histories.Any(h => h.license_hist_id == Id));

                if (parFormfield == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                return Ok(parFormfield, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetList(string? crs = null)
        //{
        //    try
        //    {
        //        int runUserID = await getDefRunUserId();
        //        string runUser = await getDefRunUser();
        //        var result = from lesen in _tenantDBContext.mst_licensees
        //                     where lesen.created_at >= DateTime.Now.AddYears(-5)
        //                     join owner in _tenantDBContext.mst_owners on lesen.owner_icno equals owner.owner_icno
        //                     join premis in _tenantDBContext.mst_premis on lesen.license_accno equals premis.lesen into premisGroup
        //                     from premis in premisGroup.DefaultIfEmpty()

        //                     group new { lesen, owner, premis } by new
        //                     {
        //                         lesen.licensee_id,
        //                         lesen.license_accno,
        //                         owner.owner_icno,
        //                         owner.owner_name,
        //                     }
        //                     into grouped
        //                     select new premis_history_view
        //                     {
        //                         gid = grouped.Key.licensee_id,
        //                         nama_pemilik = grouped.Key.owner_name,
        //                         no_ic_pemilik = grouped.Key.owner_icno,
        //                         no_lesen_bisness = grouped.Key.license_accno,
        //                         nama_perniagaan = grouped.Select(x => x.lesen.business_name)
        //                                                   .FirstOrDefault(),
        //                         alamat_premis1 = grouped.Select(x => x.lesen.business_addr)
        //                                                   .FirstOrDefault(),
        //                         jumlah_bisness = _tenantDBContext.mst_licensees
        //                                               .Count(l => l.owner_icno == grouped.Key.owner_icno),
        //                         status_lesen = grouped.Select(x => x.premis.tempoh_sah_lesen)
        //                                               .FirstOrDefault() == null
        //                                               ? "Tiada"
        //                                               : grouped.Select(x => x.premis.tempoh_sah_lesen)
        //                                                       .FirstOrDefault() > DateOnly.FromDateTime(DateTime.Now)
        //                                               ? "Aktif"
        //                                               : "Tamat Tempoh",
        //                         tempoh_sah_lesen = grouped.Select(x => x.premis.tempoh_sah_lesen)
        //                                                   .FirstOrDefault(),
        //                         tempoh_sah_cukai = grouped.Select(x => x.premis.tempoh_sah_cukai)
        //                                                   .FirstOrDefault(),
        //                         no_lesen_premis = grouped.Select(x => x.premis.lesen)
        //                                                  .FirstOrDefault(),
        //                     };


        //        return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}


        //[HttpGet]
        //public async Task<IActionResult> GetListByTabType(string tabType)
        //{
        //    try
        //    {
        //        int runUserID = await getDefRunUserId();
        //        string runUser = await getDefRunUser();

        //        if (tabType == "Premis")
        //        {
        //            var resultPremis = from owner in _tenantDBContext.mst_owners
        //                               where owner.created_at >= DateTime.Now.AddYears(-5)

        //                               select new premis_history_view
        //                               {
        //                                   strgid = owner.owner_icno,
        //                                   ssm_no = _tenantDBContext.mst_licensees
        //                                                 .Join(_tenantDBContext.mst_taxholders,
        //                                                       tax => tax.owner_icno,
        //                                                       taxType => taxType.owner_icno,
        //                                                       (tax, taxType) => new { tax, taxType })
        //                                                 .Where(x => x.tax.owner_icno == owner.owner_icno)
        //                                                 .Select(x => x.taxType.ssm_no)
        //                                                 .FirstOrDefault(),
        //                                   tax_acc_no = _tenantDBContext.mst_licensees
        //                                                 .Join(_tenantDBContext.mst_taxholders,
        //                                                       tax => tax.owner_icno,
        //                                                       taxType => taxType.owner_icno,
        //                                                       (tax, taxType) => new { tax, taxType })
        //                                                 .Where(x => x.tax.owner_icno == owner.owner_icno)
        //                                                 .Select(x => x.taxType.tax_accno)
        //                                                 .FirstOrDefault(),
        //                                   nama_pemilik = owner.owner_name,
        //                                   no_ic_pemilik = owner.owner_icno,
        //                                   alamat_premis1 = owner.owner_addr,
        //                                };
        //            var grpbyIc = from data in resultPremis
        //                          join lesen in _tenantDBContext.mst_licensees on data.no_ic_pemilik equals lesen.owner_icno
        //                          select new premis_history_view
        //                          {


        //                          };

        //            return Ok(resultPremis, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //        }
        //        else
        //        {
        //            var resultLesen = from lesen in _tenantDBContext.mst_licensees
        //                              where lesen.created_at >= DateTime.Now.AddYears(-5)
        //                              join owner in _tenantDBContext.mst_owners on lesen.owner_icno equals owner.owner_icno
        //                              join premis in _tenantDBContext.mst_premis on lesen.license_accno equals premis.lesen into premisGroup
        //                              from premis in premisGroup.DefaultIfEmpty()

        //                              join notis in _tenantDBContext.trn_notices on lesen.license_accno equals notis.license_accno into NotisGroup
        //                              from notis in NotisGroup.DefaultIfEmpty()

        //                              join kompaun in _tenantDBContext.trn_compounds on lesen.license_accno equals kompaun.license_accno into KompaunGroup
        //                              from kompaun in KompaunGroup.DefaultIfEmpty()

        //                              join sita in _tenantDBContext.trn_confiscations on lesen.license_accno equals sita.license_accno into SitaGroup
        //                              from sita in SitaGroup.DefaultIfEmpty()

        //                              join nota in _tenantDBContext.trn_inspections on lesen.owner_icno equals nota.owner_icno into NotaGroup
        //                              from nota in NotaGroup.DefaultIfEmpty()

        //                              group new { lesen, owner, premis, notis, kompaun, sita, nota } by new
        //                              {
        //                                  lesen.licensee_id,
        //                                  lesen.license_accno,
        //                                  owner.owner_icno,
        //                              }
        //                         into grouped
        //                              select new premis_history_view
        //                              {
        //                                  strgid = grouped.Key.owner_icno,
        //                                  nama_pemilik = grouped.Select(x => x.owner.owner_name)
        //                                                            .FirstOrDefault(),
        //                                  no_ic_pemilik = grouped.Key.owner_icno,
        //                                  no_lesen_bisness = grouped.Key.license_accno,
        //                                  nama_perniagaan = grouped.Select(x => x.lesen.business_name)
        //                                                            .FirstOrDefault(),
        //                                  alamat_premis1 = grouped.Select(x => x.lesen.business_addr)
        //                                                            .FirstOrDefault(),
        //                                  status_lesen = grouped.Select(x => x.premis.tempoh_sah_lesen)
        //                                                        .FirstOrDefault() == null
        //                                                        ? "Tiada Data"
        //                                                        : grouped.Select(x => x.premis.tempoh_sah_lesen)
        //                                                                .FirstOrDefault() > DateOnly.FromDateTime(DateTime.Now)
        //                                                        ? "Aktif"
        //                                                        : "Tamat Tempoh",
        //                                  tarikh_daftar = (DateTime)grouped.Select(x => x.lesen.reg_date)
        //                                                            .FirstOrDefault(),
        //                                  jumlah_notis = _tenantDBContext.trn_notices
        //                                                        .Count(l => l.license_accno == grouped.Key.license_accno),
        //                                  jumlah_kompaun = _tenantDBContext.trn_compounds
        //                                                        .Count(l => l.license_accno == grouped.Key.license_accno),
        //                                  jumlah_sita = _tenantDBContext.trn_confiscations
        //                                                        .Count(l => l.license_accno == grouped.Key.license_accno),
        //                                  jumlah_nota_pemeriksaan = _tenantDBContext.trn_inspections
        //                                                        .Count(l => l.owner_icno == grouped.Key.owner_icno),
        //                                  ops_name = _tenantDBContext.mst_licensees
        //                                                 .Join(_tenantDBContext.ref_license_ops,
        //                                                       ops => ops.ops_id,
        //                                                        opsType => opsType.ops_id,
        //                                                       (ops, opsType) => new { ops, opsType })
        //                                                 .Where(x => x.ops.license_accno == grouped.Key.license_accno)
        //                                                 .Select(x => x.opsType.ops_name)
        //                                                 .FirstOrDefault(),
        //                                  //no_rujukan_notis = grouped.Select(x => x.notis.notice_ref_no)
        //                                  //                         .FirstOrDefault(),
        //                                  //jenis_saman_notis = _tenantDBContext.trn_notices
        //                                  //               .Join(_tenantDBContext.ref_notice_types,
        //                                  //                     notice => notice.notice_type_id,
        //                                  //                     noticeType => noticeType.notice_type_id,
        //                                  //                     (notice, noticeType) => new { notice, noticeType })
        //                                  //               .Where(x => x.notice.license_accno == grouped.Key.license_accno)
        //                                  //               .Select(x => x.noticeType.notice_type_name)
        //                                  //               .FirstOrDefault(),
        //                                  //tempoh_saman_notis = _tenantDBContext.trn_notices
        //                                  //               .Join(_tenantDBContext.ref_ntc_durations,
        //                                  //                     durations => durations.duration_id,
        //                                  //                     durationsType => durationsType.duration_id,
        //                                  //                     (durations, durationsType) => new { durations, durationsType })
        //                                  //               .Where(x => x.durations.license_accno == grouped.Key.license_accno)
        //                                  //               .Select(x => x.durationsType.duration_value)
        //                                  //               .FirstOrDefault(),

        //                              };
        //            return Ok(resultLesen, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //}

    }
}
