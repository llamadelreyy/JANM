/*
Project: PBT Pro
Description: Form Field setup parameter controller
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
06/11/2024 - initial create
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ConfigFormFieldController : IBaseController
    {
        private readonly ILogger<ConfigFormFieldController> _logger;
        private readonly string _feature = "CONFIG_FORM_FIELD";

        public ConfigFormFieldController(PBTProDbContext dbContext, ILogger<ConfigFormFieldController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var parFormfields = await _dbContext.config_form_fields.Where(x => x.active_flag == true).OrderBy(x => x.field_orders).ThenBy(x => x.field_name).AsNoTracking().ToListAsync();

                if (parFormfields.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(parFormfields, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        //[Route("GetListByFormType")]
        public async Task<IActionResult> GetListByFormType(string formType)
        {
            try
            {
                var parFormfields = await _dbContext.config_form_fields.Where(x => x.active_flag == true && x.field_form_type.ToUpper() == formType.ToUpper()).OrderBy(x => x.field_orders).ThenBy(x=> x.field_name)
                                    .Select(x => new config_form_field_view{
                                        field_id = x.field_id,
                                        field_name = x.field_name,
                                        field_label = x.field_label,
                                        field_form_type = x.field_form_type,
                                        field_option = x.field_option,
                                        field_source_url = x.field_source_url,
                                        field_required = x.field_required,
                                        field_api_seeded = x.field_api_seeded,
                                        field_orders = x.field_orders,
                                        field_type = x.field_type
                                    })
                                    .AsNoTracking().ToListAsync();

                if (parFormfields.Count == 0)
                {
                    return NoContent(SystemMesg("COMMON", "EMPTY_DATA", MessageTypeEnum.Error, string.Format("Tiada rekod untuk dipaparkan")));
                }

                return Ok(parFormfields, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Id}")]
        //[Route("GetDetail")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.config_form_fields.FirstOrDefaultAsync(x => x.field_id == Id);
                
                if(parFormfield == null)
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

        [HttpPost]
        //[Route("Create")]
        public async Task<IActionResult> Add([FromBody] config_form_field_view InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                if(InputModel.field_type == "dropdown" && 
                    string.IsNullOrWhiteSpace(InputModel.field_option) &&
                    InputModel.field_api_seeded == false)
                {
                    return Error("", SystemMesg(_feature, "OPTION_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Pilihan diperlukan")));
                }

                if(InputModel.field_api_seeded == true && string.IsNullOrWhiteSpace(InputModel.field_source_url))
                {
                    return Error("", SystemMesg(_feature, "SOURCE_URL_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan URL Sumber diperlukan")));
                }

                var isExists = await _dbContext.config_form_fields.FirstOrDefaultAsync(x => x.field_name.ToUpper() == InputModel.field_name.ToUpper() && x.field_form_type == InputModel.field_type);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "FIELD_NAME_ISEXISTS", MessageTypeEnum.Error, string.Format("Nama telah wujud")));
                }
                #endregion

                config_form_field formField = new config_form_field
                {
                    field_form_type = InputModel.field_form_type,
                    field_name = InputModel.field_name,
                    field_label = InputModel.field_label,
                    field_type = InputModel.field_type,
                    field_option = InputModel.field_option,
                    field_source_url = InputModel.field_source_url,
                    field_required = InputModel.field_required,
                    field_api_seeded = InputModel.field_api_seeded,
                    field_orders = InputModel.field_orders,
                    active_flag = true,
                    created_by = runUserID,
                    created_date = DateTime.Now
                };

                _dbContext.config_form_fields.Add(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya menambah medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        //[Route("Update")]
        public async Task<IActionResult> Update(int Id, [FromBody] config_form_field_view InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.config_form_fields.FirstOrDefaultAsync(x => x.field_id == Id);
                if(formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (InputModel.field_type == "DROPDOWN" &&
                    string.IsNullOrWhiteSpace(InputModel.field_option) &&
                    InputModel.field_api_seeded == false)
                {
                    return Error("", SystemMesg(_feature, "OPTION_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Pilihan diperlukan")));
                }

                if (InputModel.field_api_seeded == true && string.IsNullOrWhiteSpace(InputModel.field_source_url))
                {
                    return Error("", SystemMesg(_feature, "SOURCE_URL_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan URL Sumber diperlukan")));
                }
                if (formField.field_name.ToUpper() != InputModel.field_name.ToUpper())
                {
                    var isExists = await _dbContext.config_form_fields.FirstOrDefaultAsync(x => x.field_name.ToUpper() == InputModel.field_name.ToUpper() && x.field_form_type == InputModel.field_type && x.field_id != Id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "FIELD_NAME_ISEXISTS", MessageTypeEnum.Error, string.Format("Nama telah wujud")));
                    }
                }
                #endregion

                formField.field_form_type = InputModel.field_form_type;
                formField.field_name = InputModel.field_name;
                formField.field_label = InputModel.field_label;
                formField.field_type = InputModel.field_type;
                formField.field_option = InputModel.field_option;
                formField.field_source_url = InputModel.field_source_url;
                formField.field_required = InputModel.field_required;
                formField.field_api_seeded = InputModel.field_api_seeded;
                formField.field_orders = InputModel.field_orders;
                formField.updated_by = runUserID;
                formField.update_date = DateTime.Now;
    
                _dbContext.config_form_fields.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Remove(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.config_form_fields.FirstOrDefaultAsync(x => x.field_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.config_form_fields.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
