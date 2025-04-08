/*
Project: PBT Pro
Description: Taxholder API controller to handle tax 
Author: ismail
Date: March 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
27/03/2025 - initial create
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class TaxholderController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaxholderController> _logger;

        private readonly string _feature = "TAXHOLDER";
        public TaxholderController(IConfiguration configuration, PBTProDbContext dbContext, PBTProTenantDbContext tntdbContext, ILogger<TaxholderController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
            _tenantDBContext = tntdbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(string tabType)
        {
            try
            {
                var result = await _tenantDBContext.mst_taxholders.Where(e => e.is_deleted == false && e.created_at >= DateTime.Now.AddYears(-5))
                     .AsNoTracking()
                     .ToListAsync();

                return Ok(result, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Transaction ticket count by
        [HttpGet("{TaxAccNo}")]
        public async Task<IActionResult> GetTicketCountByTaxAccNo(string TaxAccNo, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IQueryable<trn_inspect> initInspect = _tenantDBContext.trn_inspects.Where(x => x.tax_accno == TaxAccNo);
                IQueryable<trn_cfsc> initConfiscation = _tenantDBContext.trn_cfscs.Where(x => x.tax_accno == TaxAccNo);
                IQueryable<trn_notice> initNotice = _tenantDBContext.trn_notices.Where(x => x.tax_accno == TaxAccNo);
                IQueryable<trn_cmpd> initCompound = _tenantDBContext.trn_cmpds.Where(x => x.tax_accno == TaxAccNo);

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initInspect = initInspect.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initConfiscation = initConfiscation.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initNotice = initNotice.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initCompound = initCompound.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                int cntInspect = await initInspect.CountAsync();
                int cntConfiscation = await initConfiscation.CountAsync();
                int cntNotice = await initNotice.CountAsync();
                int cntCompound = await initCompound.CountAsync();

                var resultData = new
                {
                    inspection = cntInspect,
                    confiscation = cntConfiscation,
                    notice = cntNotice,
                    compound = cntCompound
                };

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{TaxId}")]
        public async Task<IActionResult> GetTicketCountByTaxId(int TaxId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var taxholderInfo = await _tenantDBContext.mst_taxholders.AsNoTracking().FirstOrDefaultAsync(x => x.taxholder_id == TaxId);
                if (taxholderInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                IQueryable<trn_inspect> initInspect = _tenantDBContext.trn_inspects.Where(x => x.tax_accno == taxholderInfo.tax_accno);
                IQueryable<trn_cfsc> initConfiscation = _tenantDBContext.trn_cfscs.Where(x => x.tax_accno == taxholderInfo.tax_accno);
                IQueryable<trn_notice> initNotice = _tenantDBContext.trn_notices.Where(x => x.tax_accno == taxholderInfo.tax_accno);
                IQueryable<trn_cmpd> initCompound = _tenantDBContext.trn_cmpds.Where(x => x.tax_accno == taxholderInfo.tax_accno);

                if (startDate.HasValue)
                {
                    if (!endDate.HasValue)
                    {
                        endDate = startDate;
                    }
                    initInspect = initInspect.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initConfiscation = initConfiscation.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initNotice = initNotice.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                    initCompound = initCompound.Where(x => x.created_at.Value.Date >= startDate.Value.Date && x.created_at.Value.Date <= endDate.Value.Date);
                }

                int cntInspect = await initInspect.CountAsync();
                int cntConfiscation = await initConfiscation.CountAsync();
                int cntNotice = await initNotice.CountAsync();
                int cntCompound = await initCompound.CountAsync();

                var resultData = new
                {
                    inspection = cntInspect,
                    confiscation = cntConfiscation,
                    notice = cntNotice,
                    compound = cntCompound
                };

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

    }
}
