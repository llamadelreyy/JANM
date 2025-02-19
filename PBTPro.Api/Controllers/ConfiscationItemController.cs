//using Microsoft.AspNetCore.Mvc;

//namespace PBTPro.Api.Controllers
//{
//    public class ConfiscationItemController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;


namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ConfiscationItemController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushDataHub> _hubContext;
        private readonly string _feature = "CONFISCATIONS_ITEMS";
        private readonly ILogger<ConfiscationItemController> _logger;

        public ConfiscationItemController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<ConfiscationItemController> logger, IHubContext<PushDataHub> hubContext, PBTProTenantDbContext tntdbContext) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _hubContext = hubContext;
            _tenantDBContext = tntdbContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ref_cfsc_inventory>>> ListAll()
        {
            try
            {

                var data = await (from item in _tenantDBContext.ref_cfsc_inventories
                                  join type in _tenantDBContext.ref_cfsc_invtypes on item.item_type equals type.inv_type_id
                                  where item.is_deleted == false
                                  select new
                                  {
                                      item.inv_id,
                                      item.inv_name,
                                      item.item_type,
                                      type.inv_type_desc,
                                      item.created_at,
                                      item.modified_at,
                                  }).ToListAsync();

                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _tenantDBContext.ref_cfsc_inventories.FirstOrDefaultAsync(x => x.inv_id == Id);

                if (parFormfield == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                return Ok(parFormfield, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ref_cfsc_inventory InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region store data
                ref_cfsc_inventory ref_item_confiscation = new ref_cfsc_inventory
                {
                    inv_name = InputModel.inv_name,
                    item_type = InputModel.item_type,
                    is_deleted = false,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                    modifier_id = runUserID,
                    modified_at = DateTime.Now,
                };

                _tenantDBContext.ref_cfsc_inventories.Add(ref_item_confiscation);
                await _tenantDBContext.SaveChangesAsync();

                #endregion

                var result = new
                {
                    inv_name = ref_item_confiscation.inv_name,
                    item_type = ref_item_confiscation.item_type,
                    is_deleted = ref_item_confiscation.is_deleted,
                    created_at = ref_item_confiscation.created_at
                };
                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta sitaan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ref_cfsc_inventory InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.ref_cfsc_inventories.FirstOrDefaultAsync(x => x.inv_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.inv_name))
                {
                    return Error("", SystemMesg(_feature, "INV_NAME", MessageTypeEnum.Error, string.Format("Ruangan nama barang sita diperlukan")));
                }


                #endregion

                formField.inv_name = InputModel.inv_name;
                formField.item_type = InputModel.item_type;

                formField.is_deleted = InputModel.is_deleted;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _tenantDBContext.ref_cfsc_inventories.Update(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "UPDATE", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _tenantDBContext.ref_cfsc_inventories.FirstOrDefaultAsync(x => x.inv_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _tenantDBContext.ref_cfsc_inventories.Remove(formField);
                await _tenantDBContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [AllowAnonymous]
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetConfiscationItemListByUserId(int UserId)
        {
            try
            {
                var resultData = new List<dynamic>();

                var totalCount = await _tenantDBContext.ref_cfsc_inventories
                    .Where(count => count.creator_id == UserId)
                    .CountAsync();

                var confiscation_item_lists = await (from item in _tenantDBContext.ref_cfsc_inventories
                                                     join type in _tenantDBContext.ref_cfsc_invtypes on item.item_type equals type.inv_type_id
                                                     where item.creator_id == UserId
                                                     select new
                                                     {
                                                         item.inv_id,
                                                         item.inv_name,
                                                         item.item_type,
                                                         type.inv_type_desc,
                                                         item.created_at,
                                                         item.modified_at,
                                                     }).ToListAsync();

                // Check if no record was found
                if (confiscation_item_lists.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                resultData.Add(new
                {
                    total_records = totalCount,
                    confiscation_item_lists,
                });

                return Ok(resultData, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

    }
}
