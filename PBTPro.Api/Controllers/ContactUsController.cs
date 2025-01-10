/*
Project: PBT Pro
Description: ContactUs API controller to handle Contact Us Form Field
Author: Nurulfarhana
Date: January 2025
Version: 1.0
Additional Notes:
- 
Changes Logs:
10/01/2024 - initial create
*/

using DevExpress.XtraPrinting.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ContactUsController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private string LoggerName = "administrator";
        private readonly string _feature = "HUBUNGI_KAMI";
        private readonly IEmailSender _emailSender;

        public ContactUsController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<ContactUsController> logger, IEmailSender emailSender) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<contact_us>>> ListAll()
        {
            try
            {
                var data = await _dbContext.contact_us.AsNoTracking().ToListAsync();
                return Ok(data, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetail(int Id)
        {
            try
            {
                var parFormfield = await _dbContext.contact_us.FirstOrDefaultAsync(x => x.contact_id == Id);

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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] contact_us InputModel)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region Validation
                if (string.IsNullOrWhiteSpace(InputModel.contact_name))
                {
                    return Error("", SystemMesg(_feature, "CONTACT_NAME", MessageTypeEnum.Error, string.Format("Ruangan nama diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.contact_email))
                {
                    return Error("", SystemMesg(_feature, "CONTACT_EMAIL", MessageTypeEnum.Error, string.Format("Ruangan emel  diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.contact_telno))
                {
                    return Error("", SystemMesg(_feature, "CONTACT_TEL_NO", MessageTypeEnum.Error, string.Format("Ruangan no telefon telefon diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.contact_subject))
                {
                    return Error("", SystemMesg(_feature, "CONTACT_SUBJECT", MessageTypeEnum.Error, string.Format("Ruangan subjek diperlukan")));
                }
                if (string.IsNullOrWhiteSpace(InputModel.contact_message))
                {
                    return Error("", SystemMesg(_feature, "CONTACT_MESSAGE", MessageTypeEnum.Error, string.Format("Ruangan catatan diperlukan")));
                }

                #endregion

                #region store data
                contact_us contact_us = new contact_us
                {
                    contact_inq_no = InputModel.contact_inq_no,
                    contact_name = InputModel.contact_name,
                    contact_email = InputModel.contact_email,
                    contact_telno = InputModel.contact_telno,
                    contact_subject = InputModel.contact_subject,
                    contact_message = InputModel.contact_message,
                    contact_status = InputModel.contact_status,
                    response_message = InputModel.response_message,
                    creator_id = runUserID,
                    created_at = DateTime.Now,
                };

                _dbContext.contact_us.Add(contact_us);
                await _dbContext.SaveChangesAsync();

                await SendEmailContactUs(contact_us.contact_email, contact_us.contact_name, contact_us.response_message, contact_us.contact_status);

                #endregion

                return Ok(contact_us, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya tambah rondaan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] contact_us InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.contact_us.FirstOrDefaultAsync(x => x.contact_id == InputModel.contact_id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                #endregion

                formField.contact_status = InputModel.contact_status;
                formField.response_message = InputModel.response_message;
                formField.modifier_id = runUserID;
                formField.modified_at = DateTime.Now;

                _dbContext.contact_us.Update(formField);
                await _dbContext.SaveChangesAsync();

                await SendEmailContactUs(InputModel.contact_email, InputModel.contact_name, InputModel.response_message, InputModel.contact_status);

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string runUser = await getDefRunUser();

                #region Validation
                var formField = await _dbContext.contact_us.FirstOrDefaultAsync(x => x.contact_id == Id);
                if (formField == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.contact_us.Remove(formField);
                await _dbContext.SaveChangesAsync();

                return Ok(formField, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        private bool ContactUsExists(int id)
        {
            return (_dbContext.contact_us?.Any(e => e.contact_id == id)).GetValueOrDefault();
        }

        private async Task<bool> SendEmailContactUs(string recipient, string username, string response, string status)
        {
            try
            {
                EmailContent defaultContent = new EmailContent();
                switch (status)
                {
                    case "Menunggu":
                        defaultContent = new EmailContent
                        {
                            subject = "Terima kasih atas pertanyaan anda.",
                            body = "Hai [0], Anda telah menghantar pertanyaaan anda kepada kami.<br/><br/>" +
                                    "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir PBT Pro<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                        };
                        break;
                    case "Dalam Proses":
                        defaultContent = new EmailContent
                        {
                            subject = "Terima kasih atas pertanyaan anda. Pertanyaan anda dalam proses.",
                            body = "Hai [0], Anda telah menghantar pertanyaaan anda kepada kami. Kami sedang memprosesnya. Terima kasih atas kesabaran anda. <br/><br/>" +
                                    "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir PBT Pro<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                        };
                        break;
                    case "Selesai":
                        defaultContent = new EmailContent
                        {
                            subject = "Terima kasih atas pertanyaan anda. Pertanyaan anda dalam sudah selesai.",
                            body = "Hai [0], Pertanyaan anda sudah selesai. Terima kasih atas kesabaran anda. <br/><br/>" +
                                     "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir PBT Pro<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                        };
                        break;
                    default:
                        defaultContent = new EmailContent
                        {
                            subject = "Terima kasih atas pertanyaan anda.",
                            body = "Hai [0], Anda telah menghantar pertanyaaan anda kepada kami. Kami sedang memprosesnya. Terima kasih atas kesabaran anda. <br/><br/>" +
                                    "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir PBT Pro<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                        };
                        break;
                }

                string[] param = { username, response, status };

                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("CONTACT_US", param, defaultContent);

                var emailRs = await emailHelper.QueueEmail(emailContent.subject, emailContent.body, recipient);
                var sentRs = await emailHelper.ForceProcessQueue(emailRs);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
