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
using Newtonsoft.Json;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models.CommonService;
using PBTPro.Shared.Models.RequestPayLoad;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [AllowAnonymous]
    public class ParamFormFieldController : IBaseController
    {
        private readonly ILogger<ParamFormFieldController> _logger;
        private readonly string _feature = "PARAM_FORM_FIELD";

        public ParamFormFieldController(PBTProDbContext dbContext, ILogger<ParamFormFieldController> logger) : base(dbContext)
        {
            _logger = logger;
        }

        [HttpGet]
        //[Route("GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var parFormfields = await _dbContext.ParFormFields.Where(x => x.Isactive == true).OrderBy(x => x.Orders).ThenBy(x => x.Name).AsNoTracking().ToListAsync();

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
                var parFormfields = await _dbContext.ParFormFields.Where(x => x.Isactive == true && x.FormType.ToUpper() == formType.ToUpper()).OrderBy(x => x.Orders).ThenBy(x=> x.Name)
                                    .Select(x => new SetupBorangListModel{
                                        RecId = x.RecId, 
                                        Name = x.Name, 
                                        Label = x.Label, 
                                        Type = x.Type,
                                        Option = x.Option,
                                        SourceUrl = x.SourceUrl,
                                        Required = x.Required,
                                        ApiSeeded = x.ApiSeeded,
                                        Orders = x.Orders
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
                var parFormfield = await _dbContext.ParFormFields.FirstOrDefaultAsync(x => x.RecId == Id);
                
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
        public async Task<IActionResult> Create([FromBody] ParamFormFieldInputModel InputModel)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                if(InputModel.Type == "dropdown" && 
                    string.IsNullOrWhiteSpace(InputModel.Option) &&
                    InputModel.ApiSeeded == false)
                {
                    return Error("", SystemMesg(_feature, "OPTION_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Pilihan diperlukan")));
                }

                if(InputModel.ApiSeeded == true && string.IsNullOrWhiteSpace(InputModel.SourceUrl))
                {
                    return Error("", SystemMesg(_feature, "SOURCE_URL_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan URL Sumber diperlukan")));
                }

                var isExists = await _dbContext.ParFormFields.FirstOrDefaultAsync(x => x.Name.ToUpper() == InputModel.Name.ToUpper() && x.FormType == InputModel.FormType);
                if (isExists != null)
                {
                    return Error("", SystemMesg(_feature, "FIELD_NAME_ISEXISTS", MessageTypeEnum.Error, string.Format("Nama telah wujud")));
                }
                #endregion

                ParFormField formField = new ParFormField
                {
                    FormType = InputModel.FormType,
                    Name = InputModel.Name,
                    Label = InputModel.Label,
                    Type = InputModel.Type,
                    Option = InputModel.Option,
                    SourceUrl = InputModel.SourceUrl,
                    Required = InputModel.Required,
                    ApiSeeded = InputModel.ApiSeeded,
                    Orders = InputModel.Orders,
                    Isactive = true,
                    CreatedBy = runUser,
                    CreatedDtm = DateTime.Now
                };

                _dbContext.ParFormFields.Add(formField);
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
        public async Task<IActionResult> Update(int Id, [FromBody] ParamFormFieldInputModel InputModel)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ParFormFields.FirstOrDefaultAsync(x => x.RecId == Id);
                if(formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (InputModel.Type == "DROPDOWN" &&
                    string.IsNullOrWhiteSpace(InputModel.Option) &&
                    InputModel.ApiSeeded == false)
                {
                    return Error("", SystemMesg(_feature, "OPTION_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan Pilihan diperlukan")));
                }

                if (InputModel.ApiSeeded == true && string.IsNullOrWhiteSpace(InputModel.SourceUrl))
                {
                    return Error("", SystemMesg(_feature, "SOURCE_URL_ISREQUIRED", MessageTypeEnum.Error, string.Format("Ruangan URL Sumber diperlukan")));
                }
                if (formField.Name.ToUpper() != InputModel.Name.ToUpper())
                {
                    var isExists = await _dbContext.ParFormFields.FirstOrDefaultAsync(x => x.Name.ToUpper() == InputModel.Name.ToUpper() && x.FormType == InputModel.FormType && x.RecId != Id);
                    if (isExists != null)
                    {
                        return Error("", SystemMesg(_feature, "FIELD_NAME_ISEXISTS", MessageTypeEnum.Error, string.Format("Nama telah wujud")));
                    }
                }
                #endregion

                formField.FormType = InputModel.FormType;
                formField.Name = InputModel.Name;
                formField.Label = InputModel.Label;
                formField.Type = InputModel.Type;
                formField.Option = InputModel.Option;
                formField.SourceUrl = InputModel.SourceUrl;
                formField.Required = InputModel.Required;
                formField.ApiSeeded = InputModel.ApiSeeded;
                formField.Orders = InputModel.Orders;
                formField.ModifiedBy = runUser;
                formField.ModifiedDtm = DateTime.Now;
    
                _dbContext.ParFormFields.Update(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        //[Route("Remove")]
        public async Task<IActionResult> Remove(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.ParFormFields.FirstOrDefaultAsync(x => x.RecId == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.ParFormFields.Remove(formField);
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
