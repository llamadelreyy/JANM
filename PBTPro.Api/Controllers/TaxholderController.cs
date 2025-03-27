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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class TaxholderController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IHubContext<PushDataHub> _hubContext;
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
        [HttpGet("{LicenseAccNo}")]
        public async Task<IActionResult> GetTicketCountByTaxAccNo(string TaxAccNo)
        {
            try
            {
                int cntInspect = await _tenantDBContext.trn_inspects.Where(x => x.tax_accno == TaxAccNo).CountAsync();
                int cntConfiscation = await _tenantDBContext.trn_cfscs.Where(x => x.tax_accno == TaxAccNo).CountAsync();
                int cntNotice = await _tenantDBContext.trn_notices.Where(x => x.tax_accno == TaxAccNo).CountAsync();
                int cntCompound = await _tenantDBContext.trn_cmpds.Where(x => x.tax_accno == TaxAccNo).CountAsync();

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
        public async Task<IActionResult> GetTicketCountByTaxId(int TaxId)
        {
            try
            {
                var taxholderInfo = await _tenantDBContext.mst_taxholders.AsNoTracking().FirstOrDefaultAsync(x => x.taxholder_id == TaxId);
                if (taxholderInfo == null)
                {
                    return Error("", SystemMesg(_feature, "LICENSENO_INVALID", MessageTypeEnum.Error, string.Format("no akaun lesen tidak sah")));
                }

                int cntInspect = await _tenantDBContext.trn_inspects.Where(x => x.tax_accno == taxholderInfo.tax_accno).CountAsync();
                int cntConfiscation = await _tenantDBContext.trn_cfscs.Where(x => x.tax_accno == taxholderInfo.tax_accno).CountAsync();
                int cntNotice = await _tenantDBContext.trn_notices.Where(x => x.tax_accno == taxholderInfo.tax_accno).CountAsync();
                int cntCompound = await _tenantDBContext.trn_cmpds.Where(x => x.tax_accno == taxholderInfo.tax_accno).CountAsync();

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
