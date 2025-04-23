/*
Project: PBT Pro
Description: Dashboard ZonEksekutif API controller to handle reporting
Author: Nurulfarhana
Date: November 2024
Version: 1.0
Additional Notes:
- 
Changes Logs:
14/11/2024 - initial create
Ticket No: 053
*/
using DevExpress.Data.Linq.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Data;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DashboardController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        protected PBTProTenantDbContext _tenantDBContext;
        private readonly ILogger<DashboardController> _logger;
        private readonly string _feature = "DASHBOARD";

        public DashboardController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<DashboardController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }


        #region jenis-jenis saman

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<dashboard_view>> GetDashboardData()
        {
            try
            {
                var totalNotis = await _tenantDBContext.trn_notices.CountAsync();
                var totalKompaun = await _tenantDBContext.trn_cmpds.CountAsync();
                var totalNota = await _tenantDBContext.trn_inspects.CountAsync();
                var totalSita = await _tenantDBContext.trn_cfscs.CountAsync();
                var premisBerlesen = await _tenantDBContext.mst_license_premis_taxes.Where(t => t.status_lesen_id == 1).CountAsync();

                var premisTindakan = await _tenantDBContext.mst_owner_premis
                    .Select(lesen => new
                    {
                        lesen.owner_icno,
                        NoticeCount = _tenantDBContext.trn_notices.Count(notis => notis.owner_icno == lesen.owner_icno),
                        CompoundCount = _tenantDBContext.trn_cmpds.Count(kompaun => kompaun.owner_icno == lesen.owner_icno),
                        ConfiscationCount = _tenantDBContext.trn_cfscs.Count(sita => sita.owner_icno == lesen.owner_icno)
                    })
                    .ToListAsync();

                var totalPremisTindakan = premisTindakan.Sum(x => x.NoticeCount + x.CompoundCount + x.ConfiscationCount);
                var premisTiadaLesen = await _tenantDBContext.mst_license_premis_taxes.Where(p => p.status_lesen_id == 4).CountAsync(); // await _tenantDBContext.mst_premis.Where(p => p.lesen == null || p.lesen == "" || string.IsNullOrEmpty(p.lesen)).CountAsync();
                var premisTamatTempohLesen = await _tenantDBContext.mst_license_premis_taxes.Where(p => p.status_lesen_id == 2).CountAsync();// await _tenantDBContext.mst_premis.Where(p => p.tempoh_sah_lesen <= DateOnly.FromDateTime(DateTime.Today)).CountAsync();

                var bilCukaiTahunan = await _tenantDBContext.mst_premis.CountAsync();

                ///added by farhana 19/2/2025
                ///pending to get information to sum up :
                //var totalCukaiTaksiranByr 
                //var totalCukaiTaksiranblmByr
                //var totalHslLesenByr
                //var totalHslLesenBlmByr

                var totalKompaunDibyr = await _tenantDBContext.trn_cmpds.Where(c => c.trnstatus_id == 5).SumAsync(c => c.amt_cmpd);
                var totalKompaunBlmByr = await _tenantDBContext.trn_cmpds.Where(xc => xc.trnstatus_id == 4).SumAsync(c => c.amt_cmpd);

                ///added by farhana 13/3/2025
                var totalPremisDilawat = await _tenantDBContext.mst_patrol_schedules.Where(x => x.status_id == 1).CountAsync(); // Selesai                                                                                                                                
                var premisDiperiksa = await _tenantDBContext.mst_patrol_schedules.Where(t => t.status_id == 2).CountAsync(); //Dalam Rondaan
                var totalLokasiBaru = await _tenantDBContext.mst_patrol_schedules.Where(x => x.status_id == 3).CountAsync(); // Belum Meronda

                var lesenTindakan = await _tenantDBContext.mst_owner_licensees
                   .Select(lesen => new
                   {
                       lesen.owner_icno,
                       NoticeCount = _tenantDBContext.trn_notices.Count(notis => notis.owner_icno == lesen.owner_icno),
                       CompoundCount = _tenantDBContext.trn_cmpds.Count(kompaun => kompaun.owner_icno == lesen.owner_icno),
                       ConfiscationCount = _tenantDBContext.trn_cfscs.Count(sita => sita.owner_icno == lesen.owner_icno)
                   })
                   .ToListAsync();

                var totalLesenTindakan = lesenTindakan.Sum(x => x.NoticeCount + x.CompoundCount + x.ConfiscationCount);

                var result = new dashboard_view
                {
                    total_notis = totalNotis,
                    total_kompaun = totalKompaun,
                    total_inspection = totalNota,
                    total_confiscation = totalSita,
                    premis_berlesen = premisBerlesen,
                    premis_diperiksa = premisDiperiksa,
                    premis_dikenakan_tindakan = totalPremisTindakan,
                    premis_tiada_lesen = premisTiadaLesen,
                    premis_tamat_tempoh_lesen = premisTamatTempohLesen,
                    total_cukai_tahunan = bilCukaiTahunan,
                    amaun_kutipan_cukai = 4569.93M,
                    cukai_taksiran_dibyr = 0,// 100230,
                    cukai_taksiran_blm_dibyr = 0, //10230,
                    hsl_lesen_dibyr = 0,//150230,
                    hsl_lesen_blm_dibyr = 0,//15230,
                    kompaun_dibyr = totalKompaunDibyr,
                    kompaun_blm_dibyr = totalKompaunBlmByr,
                    total_premis_dilawat = totalPremisDilawat,
                    total_rondaan_dibuat = premisDiperiksa,
                    total_lokasi_baru = totalLokasiBaru,
                    lesen_dikenakan_tindakan = totalLesenTindakan,
                };

                if (result == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<dashboard_view>> GetFinancialDashboard()
        {
            try
            {
                var totalLesenAktif = await _tenantDBContext.mst_licensees.Where(l => l.status_id == 1).CountAsync();
                var totalLesenTamatTempoh = await _tenantDBContext.mst_licensees.Where(l => l.status_id == 2).CountAsync();
                var totalPremisPerniagaan = await _tenantDBContext.mst_premis.CountAsync();//await _tenantDBContext.mst_licensees.GroupBy(x => x.owner_icno).CountAsync();
                var pertambahanLsnThnSemasa = 10;// await _tenantDBContext.mst_licensees.Where(t => t.reg_date.HasValue && t.reg_date.Value.Year == DateTime.Now.Year).CountAsync();
                var pertambahanLsnSemasa = 20;// await _tenantDBContext.mst_licensees.Where(t => t.reg_date.HasValue&& t.reg_date.Value.Year == DateTime.Now.Year && t.reg_date.Value.Month == DateTime.Now.Month).CountAsync();

                var result = new dashboard_view
                {
                    total_lesen_aktif = totalLesenAktif,
                    lesen_tamat_tempoh = totalLesenTamatTempoh,
                    total_premis_perniagaan = totalPremisPerniagaan,
                    hsl_tahunan_semasa = 0,//202543.00M,
                    ptmbahan_lesen_thn_semasa = pertambahanLsnThnSemasa,
                    ptmbahan_lesen_semasa = pertambahanLsnSemasa,
                };

                if (result == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        #region Graph
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalNoticeGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_notices
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)  
                    .GroupBy(l => l.created_at.Value.Month) 
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalCompoundGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_cmpds
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalInspectionGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_inspects
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalConfiscationGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_cfscs
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalPaidCompoundGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_cmpds.Where(c => c.trnstatus_id == 5)
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalNotPaidCompoundGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.trn_cmpds
                    .Where(c => c.trnstatus_id == 4)
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalVisitedGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.mst_patrol_schedules
                    .Where(c => c.status_id == 1)
                    .Where(l => l.created_at.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalOngoingPatrolGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.mst_patrol_schedules
                    .Where(c => c.status_id == 2)
                    .Where(l => l.created_at.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalNewPatrolGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();
                var noticesByMonth = await _tenantDBContext.mst_patrol_schedules
                    .Where(c => c.status_id == 3)
                    .Where(l => l.created_at.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = noticesByMonth.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalPremisTakenActionGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();

                var CountTakenAction = await _tenantDBContext.mst_owner_premis
                    .Select(premis => new
                    {
                        premis.owner_icno,
                        NoticeCount = _tenantDBContext.trn_notices.Count(notis => notis.owner_icno == premis.owner_icno),
                        CompoundCount = _tenantDBContext.trn_cmpds.Count(kompaun => kompaun.owner_icno == premis.owner_icno),
                        ConfiscationCount = _tenantDBContext.trn_cfscs.Count(sita => sita.owner_icno == premis.owner_icno),
                        created_at = premis.created_at
                    })
                    .ToListAsync();

                var PremisTakenAction = CountTakenAction
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Sum(x => x.NoticeCount + x.CompoundCount + x.ConfiscationCount)
                    })
                    .ToList();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = PremisTakenAction.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<graph_report_view>> GetTotalLicenseTakenActionGraph()
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12).ToList();

                var CountTakenAction = await _tenantDBContext.mst_owner_licensees
                    .Select(license => new
                    {
                        license.owner_icno,
                        NoticeCount = _tenantDBContext.trn_notices.Count(notis => notis.owner_icno == license.owner_icno),
                        CompoundCount = _tenantDBContext.trn_cmpds.Count(kompaun => kompaun.owner_icno == license.owner_icno),
                        ConfiscationCount = _tenantDBContext.trn_cfscs.Count(sita => sita.owner_icno == license.owner_icno),
                        created_at = license.created_at 
                    })
                    .ToListAsync();

                var LicenseTakenAction = CountTakenAction
                    .Where(l => l.created_at.HasValue && l.created_at.Value.Year == DateTime.Now.Year)
                    .GroupBy(l => l.created_at.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Sum(x => x.NoticeCount + x.CompoundCount + x.ConfiscationCount)
                    })
                    .ToList();

                var result = allMonths.Select(month => new
                {
                    Month = month,
                    Count = LicenseTakenAction.FirstOrDefault(n => n.Month == month)?.Count ?? 0
                }).ToList();

                var viewResult = new graph_report_view
                {
                    total = result.Select(r => r.Count).ToList(),
                    month = result.Select(r => r.Month).ToList(),
                };

                if (result == null || result.Count == 0)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion


        #region stoc proc example       

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> JumlahNotisSP()
        //{
        //    try
        //    {
        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT tenant.func_totalnotices()", myConn))
        //            {
        //                myConn.Open();

        //                var total = myCmd.ExecuteScalar();

        //                if (total == null)
        //                {
        //                    return Error("", SystemMesg("COMMON", "NO_DATA", MessageTypeEnum.Error, "No data found."));
        //                }
        //                int totalCount = Convert.ToInt32(total);

        //                return Ok(new { totalCount = totalCount }, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, "Senarai rekod berjaya dijana"));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //    finally
        //    {
        //    }
        //}
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> JumlahPremisBerlesenSP()
        //{
        //    try
        //    {
        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("SELECT tenant.func_totalpremislicense()", myConn))
        //            {
        //                myConn.Open();

        //                var total = myCmd.ExecuteScalar();

        //                if (total == null)
        //                {
        //                    return Error("", SystemMesg("COMMON", "NO_DATA", MessageTypeEnum.Error, "No data found."));
        //                }
        //                int totalCount = Convert.ToInt32(total);

        //                return Ok(new { totalCount = totalCount }, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, "Senarai rekod berjaya dijana"));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
        //        return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
        //    }
        //    finally
        //    {
        //    }
        //}
        #endregion

    }
}

