/*
Project: PBT Pro
Description: user api, to handle user related action
Author: ismail
Date: November 2024
Version: 1.0
Additional Notes:
- 

Changes Logs:
15/11/2024 - initial create
18/11/2024 - add field & logic for signature
20/11/2024 - add field & logic for profile avatar
03/12/2024 - change hardcoded upload path & url to refer param table 
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.Internal.Postgres;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Data;
using static System.Collections.Specialized.BitVector32;
using System.Reactive;
using PBTPro.Api.Services;
using DevExpress.XtraPrinting.Export;
using System.Reflection;
using System.IO;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class UserController : IBaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityOptions _identityOptions;
        private readonly string _feature = "USER";
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly List<string> _imageFileExt = new List<string> { ".jpg", ".jpeg", ".png" };
        private readonly IEmailSender _emailSender;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;

        public UserController(PBTProDbContext dbContext, ILogger<UserController> logger, UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> identityOptions, IEmailSender emailSender, IPasswordValidator<ApplicationUser> passwordValidator) : base(dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _identityOptions = identityOptions.Value;
            _emailSender = emailSender;
            SetTenantDbContext("tenant");
            _passwordValidator = passwordValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetListUser()
        {
            try
            {
                var users = await _dbContext.Users.AsNoTracking().ToListAsync();

                return Ok(users, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var users = await _dbContext.Users.AsNoTracking().ToListAsync();
                var dtDepartment = await _dbContext.ref_departments.AsNoTracking().ToListAsync();
                var dtSection = await _dbContext.ref_divisions.AsNoTracking().ToListAsync();
                var dtUnit = await _dbContext.ref_units.AsNoTracking().ToListAsync();
                List<RegisterModel> registerModels = new List<RegisterModel>();

                if (users.Count() != 0)
                {
                    registerModels = (from user in users
                                      join dept in dtDepartment on user.dept_id equals dept.dept_id
                                      join div in dtSection on user.div_id equals div.div_id
                                      join unit in dtUnit on user.unit_id equals unit.unit_id
                                      where user.IsDeleted == false
                                      select new RegisterModel
                                      {
                                          Id = user.Id,
                                          FullName = user.full_name,
                                          Username = user.UserName,
                                          Email = user.Email,
                                          PhoneNo = user.PhoneNumber,
                                          DepartmentName = dept.dept_name,
                                          DivisionName = div.div_name,
                                          UnitName = unit.unit_name,
                                          CreatedAt = user.CreatedAt,
                                          ICNo = user.IdNo,
                                          DepartmentID = dept.dept_id,
                                          DivisionID = div.div_id,
                                          UnitID = unit.unit_id,


                                      }).ToList();
                }

                return Ok(registerModels, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var UserId = await getDefRunUserId();
                user_profile_view data = new user_profile_view();

                var baseImageViewURL = await getImageViewUrl();
                var AvatarViewURL = baseImageViewURL + "/profile";
                var SignatureViewURL = baseImageViewURL + "/signature";

                var UserProfile = await _dbContext.Users.Where(x => x.Id == UserId)
                .Select(x => new user_profile_view
                {
                    user_id = x.Id,
                    user_name = x.UserName,
                    unit_id = x.unit_id,
                    div_id = x.div_id,
                    dept_id = x.dept_id,
                    full_name = x.full_name,
                    idno = x.IdNo,
                    photo_filename = x.PhotoFilename,
                    sign_filename = x.SignFilename,
                    email = x.Email,
                    phone_number = x.PhoneNumber,
                    last_login = x.LastLogin,
                    photo_path_url = !string.IsNullOrWhiteSpace(x.PhotoFilename) ? AvatarViewURL + "/" + x.PhotoFilename : AvatarViewURL + "/avatar-user-profile-icon.jpg",
                    sign_path_url = !string.IsNullOrWhiteSpace(x.SignFilename) ? SignatureViewURL + "/" + x.SignFilename : null
                }).AsNoTracking().FirstOrDefaultAsync();

                UserProfile.dept_name = await _tenantDBContext.ref_departments.Where(x => x.dept_id == UserProfile.dept_id).Select(x => x.dept_name).FirstOrDefaultAsync();
                UserProfile.div_name = await _tenantDBContext.ref_divisions.Where(x => x.div_id == UserProfile.div_id).Select(x => x.div_name).FirstOrDefaultAsync();
                UserProfile.unit_name = await _tenantDBContext.ref_units.Where(x => x.unit_id == UserProfile.unit_id).Select(x => x.unit_name).FirstOrDefaultAsync();
                UserProfile.user_roles = await _dbContext.UserRoles.Where(x => x.UserId == UserProfile.user_id).Join(
                    _dbContext.Roles,
                    userRole => userRole.RoleId,
                    roles => roles.Id,
                    (userRole, roles) => new user_profile_role
                    {
                        Id = userRole.RoleId,
                        Name = roles.Name,
                        IsDefaultRole = userRole.IsDefaultRole,
                    }
                ).AsNoTracking().ToListAsync();

                return Ok(UserProfile, SystemMesg(_feature, "VIEW_USER_PROFILE", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSignature([FromForm] update_signature_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                bool isNew = false;

                if (runUserID != InputModel.user_id)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (!IsFileExtensionAllowed(InputModel.sign_image, _imageFileExt))
                {
                    var imageFileExtString = String.Join(", ", _imageFileExt.ToList());
                    List<string> param = new List<string> { imageFileExtString };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong [0]."), param));
                }

                if (!IsFileSizeWithinLimit(InputModel.sign_image, _maxFileSize))
                {
                    List<string> param = new List<string> { FormatFileSize(_maxFileSize) };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                }

                ApplicationUser? userProfile = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == runUserID);
                /*
                if (userProfile == null)
                {
                    isNew = true;
                    userProfile = await _dbContext.Users.Where(x => x.Id == runUserID).Select(x => new user_profile
                    {
                        user_id = x.Id,
                        profile_name = x.UserName,
                        profile_email = x.Email,
                        profile_icno = x.IdNo,
                        profile_telno = x.PhoneNumber
                    }).AsNoTracking().FirstOrDefaultAsync();
                }
                */
                #endregion

                string? FileName = userProfile.PhotoFilename;
                //D:\Workspace\Dotnet\New\PBTPro.Server\wwwroot\images\signature
                if (InputModel.sign_image?.Length > 0)
                {
                    string ImageUploadExt = Path.GetExtension(InputModel.sign_image.FileName).ToString().ToLower();

                    FileName = $"{runUserID}_{runUser}{ImageUploadExt}";
                    var UploadPath = await getImageUploadPath("signature");
                    var Fullpath = Path.Combine(UploadPath, FileName);
                    using (var stream = new FileStream(Fullpath, FileMode.Create))
                    {
                        await InputModel.sign_image.CopyToAsync(stream);
                    }

                    userProfile.SignFilename = FileName;

                    if (isNew == true)
                    {
                        userProfile.CreatorId = runUserID;
                        userProfile.CreatedAt = DateTime.Now;
                        _dbContext.Users.Add(userProfile);
                    }
                    else
                    {
                        userProfile.ModifierId = runUserID;
                        userProfile.ModifiedAt = DateTime.Now;
                        _dbContext.Users.Update(userProfile);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "UPDATE_SIGNATURE", MessageTypeEnum.Success, string.Format("Tandatangan berjaya disimpan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar([FromForm] update_avatar_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                bool isNew = false;

                if (runUserID != InputModel.user_id)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (!IsFileExtensionAllowed(InputModel.avatar_image, _imageFileExt))
                {
                    var imageFileExtString = String.Join(", ", _imageFileExt.ToList());
                    List<string> param = new List<string> { imageFileExtString };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_EXT", MessageTypeEnum.Error, string.Format("Sambungan fail tidak disokong. Jenis yang disokong ([0])."), param));
                }

                if (!IsFileSizeWithinLimit(InputModel.avatar_image, _maxFileSize))
                {
                    List<string> param = new List<string> { FormatFileSize(_maxFileSize) };
                    return Error("", SystemMesg(_feature, "INVALID_FILE_SIZE", MessageTypeEnum.Error, string.Format("saiz fail melebihi had yang dibenarkan, saiz fail maksimum yang dibenarkan ialah [0]."), param));
                }

                ApplicationUser? userProfile = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == runUserID);
                /*
                if (userProfile == null)
                {
                    isNew = true;
                    userProfile = await _dbContext.Users.Where(x => x.Id == runUserID).Select(x => new user_profile
                    {
                        user_id = x.Id,
                        profile_name = x.UserName,
                        profile_email = x.Email,
                        profile_icno = x.IdNo,
                        profile_telno = x.PhoneNumber
                    }).AsNoTracking().FirstOrDefaultAsync();
                }
                */
                #endregion

                string? FileName = userProfile.PhotoFilename;
                //D:\Workspace\Dotnet\New\PBTPro.Server\wwwroot\images\signature
                if (InputModel.avatar_image?.Length > 0)
                {
                    string ImageUploadExt = Path.GetExtension(InputModel.avatar_image.FileName).ToString().ToLower();

                    FileName = $"{runUserID}_{runUser}{ImageUploadExt}";
                    var UploadPath = await getImageUploadPath("profile");
                    var Fullpath = Path.Combine(UploadPath, FileName);
                    using (var stream = new FileStream(Fullpath, FileMode.Create))
                    {
                        await InputModel.avatar_image.CopyToAsync(stream);
                    }

                    userProfile.PhotoFilename = FileName;

                    if (isNew == true)
                    {
                        userProfile.CreatorId = runUserID;
                        userProfile.CreatedAt = DateTime.Now;
                        _dbContext.Users.Add(userProfile);
                    }
                    else
                    {
                        userProfile.ModifierId = runUserID;
                        userProfile.ModifiedAt = DateTime.Now;
                        _dbContext.Users.Update(userProfile);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "UPDATE_AVATAR", MessageTypeEnum.Success, string.Format("Gambar profil berjaya disimpan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] update_password_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                var user = await _userManager.FindByNameAsync(runUser);

                if (string.IsNullOrWhiteSpace(InputModel.new_password))
                {
                    return Error("", SystemMesg(_feature, "NEW_PASSWORD_ISNULL", MessageTypeEnum.Error, string.Format("Kata Laluan baharu diperlukan")));
                }

                if (string.IsNullOrWhiteSpace(InputModel.valid_new_password))
                {
                    return Error("", SystemMesg(_feature, "VALID_NEW_PASS_ISNULL", MessageTypeEnum.Error, string.Format("Sahkan Kata Laluan diperlukan")));
                }

                if (InputModel.new_password != InputModel.valid_new_password)
                {
                    return Error("", SystemMesg(_feature, "NEW_PASS_MISSMATCH", MessageTypeEnum.Error, string.Format("Kata Laluan baharu tidak sepadan")));
                }

                List<string> passwordErrors = new List<string>();
                var validators = _userManager.PasswordValidators;
                foreach (var validator in validators)
                {
                    var result = await validator.ValidateAsync(_userManager, null, InputModel.new_password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            if (error.Code.ToLower() == "passwordtooshort")
                            {
                                var requiredPasswordLength = _identityOptions.Password.RequiredLength;
                                List<string> param = new List<string> { requiredPasswordLength.ToString() };
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_TOO_SHORT", MessageTypeEnum.Error, string.Format("Kata laluan mestilah sekurang-kurangnya [0] aksara."), param));
                                continue;
                            }

                            if (error.Code.ToLower() == "passwordrequiresdigit")
                            {
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_REQUIRED_DIGIT", MessageTypeEnum.Error, string.Format("Kata laluan mesti mempunyai sekurang-kurangnya satu digit ('0'-'9').")));
                                continue;
                            }

                            if (error.Code.ToLower() == "passwordrequiresnonalphanumeric")
                            {
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_REQUIRED_NONALPHA", MessageTypeEnum.Error, string.Format("Kata laluan mesti mempunyai sekurang-kurangnya satu aksara bukan abjad angka.")));
                                continue;
                            }

                            if (error.Code.ToLower() == "passwordrequiresuniquechars")
                            {
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_REQUIRED_UNIQUE", MessageTypeEnum.Error, string.Format("Kata laluan mesti mempunyai sekurang-kurangnya satu aksara unik.")));
                                continue;
                            }

                            if (error.Code.ToLower() == "passwordrequireslower")
                            {
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_REQUIRED_LOWER", MessageTypeEnum.Error, string.Format("Kata laluan mesti mempunyai sekurang-kurangnya satu huruf kecil ('a'-'z').")));
                                continue;
                            }

                            if (error.Code.ToLower() == "passwordrequiresupper")
                            {
                                passwordErrors.Add(SystemMesg("AUTH", "PASSWORD_REQUIRED_UPPER", MessageTypeEnum.Error, string.Format("Kata laluan mesti mempunyai sekurang-kurangnya satu huruf besar ('A'-'Z').")));
                                continue;
                            }
                        }
                    }
                }
                if (passwordErrors.Count > 0)
                {
                    var ValidationErr = String.Join("\r\n- ", passwordErrors.ToList());
                    List<string> param = new List<string> { "- " + ValidationErr };
                    return Error("", SystemMesg(_feature, "INVALID_PASS_COMBINATION", MessageTypeEnum.Error, string.Format("Kombinasi katalaluan tidak diterima :\r\n[0]"), param));
                }
                #endregion

                var resultRM = await _userManager.RemovePasswordAsync(user);
                if (resultRM != null && resultRM.Succeeded)
                {
                    var resultADD = await _userManager.AddPasswordAsync(user, InputModel.new_password);

                    if (resultADD != null && resultADD.Succeeded)
                    {
                        user.PwdUpdateAt = DateTime.Now;
                        user.ModifierId = runUserID;
                        user.ModifiedAt = DateTime.Now;
                        await _dbContext.SaveChangesAsync();
                        _dbContext.Users.Update(user);
                        return Ok("", SystemMesg(_feature, "UPDATE_PASSWORD", MessageTypeEnum.Success, string.Format("Berjaya mengemaskini kata laluan")));
                    }
                    else
                    {
                        return Error("", SystemMesg(_feature, "UPDATE_PASSWORD", MessageTypeEnum.Error, string.Format("Gagal mengemaskini kata laluan")));
                    }
                }
                else
                {
                    return Error("", SystemMesg(_feature, "UPDATE_PASSWORD", MessageTypeEnum.Error, string.Format("Gagal mengemaskini kata laluan")));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] update_profile_input_model InputModel)
        {
            try
            {
                int runUserID = await getDefRunUserId();
                string runUser = await getDefRunUser();

                #region Validation
                bool isNew = false;

                if (runUserID != InputModel.user_id)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                ApplicationUser? userProfile = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == runUserID);
                if (userProfile == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                #region User
                if (InputModel.full_name != null) userProfile.full_name = InputModel.full_name;
                if (InputModel.idno != null) userProfile.IdNo = InputModel.idno;
                if (InputModel.email != null) userProfile.Email = InputModel.email;
                if (InputModel.phone_number != null) userProfile.PhoneNumber = InputModel.phone_number;
                if (InputModel.photo_path_url != null) userProfile.PhotoPathUrl = InputModel.photo_path_url;
                if (InputModel.photo_filename != null) userProfile.PhotoFilename = InputModel.photo_filename;
                if (InputModel.sign_filename != null) userProfile.SignFilename = InputModel.sign_filename;
                if (InputModel.dept_id.HasValue) userProfile.dept_id = InputModel.dept_id.Value;
                if (InputModel.div_id.HasValue) userProfile.div_id = InputModel.div_id.Value;
                if (InputModel.unit_id.HasValue) userProfile.unit_id = InputModel.unit_id.Value;
                userProfile.ModifierId = runUserID;
                userProfile.ModifiedAt = DateTime.Now;
                _dbContext.Users.Update(userProfile);
                #endregion

                #region Default Role
                if (InputModel.selected_role.HasValue)
                {
                    var user_roles = await _dbContext.UserRoles.Where(x => x.UserId == userProfile.Id).ToListAsync();

                    var currDef = user_roles.FirstOrDefault(x => x.IsDefaultRole == true);
                    var newDef = user_roles.FirstOrDefault(x => x.RoleId == InputModel.selected_role);

                    if (currDef != null && currDef.RoleId != newDef.RoleId)
                    {
                        currDef.IsDefaultRole = false;
                        currDef.ModifiedAt = DateTime.Now;
                        currDef.ModifierId = runUserID;
                        _dbContext.UserRoles.Update(currDef);

                        newDef.IsDefaultRole = true;
                        newDef.ModifiedAt = DateTime.Now;
                        newDef.ModifierId = runUserID;
                        _dbContext.UserRoles.Update(newDef);
                    }
                    if (currDef == null)
                    {
                        newDef.IsDefaultRole = true;
                        newDef.ModifiedAt = DateTime.Now;
                        newDef.ModifierId = runUserID;
                        _dbContext.UserRoles.Update(newDef);
                    }
                }

                #endregion
                await _dbContext.SaveChangesAsync();

                return Ok("", SystemMesg(_feature, "UPDATE_PROFILE", MessageTypeEnum.Success, string.Format("Berjaya mengemaskini profil")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMenuPermission()
        {
            try
            {
                List<AuthenticatedMenuPermission> PermissionMenus = new List<AuthenticatedMenuPermission>();
                int UserId = await getDefRunUserId();

                var userRoles = await _dbContext.UserRoles.Where(x => x.UserId == UserId).AsNoTracking().ToListAsync();

                if (userRoles.Any())
                {
                    PermissionMenus = userRoles
                    .Join(_dbContext.permissions, ur => ur.RoleId, p => p.role_id, (ur, p) => new { ur, p })
                    .Join(_dbContext.menus, combined => combined.p.menu_id, m => m.menu_id, (combined, m) => new AuthenticatedMenuPermission
                    {
                        menu_id = combined.p.menu_id,
                        menu_name = m.menu_name,
                        menu_path = m.menu_path,
                        can_view = combined.p.can_view,
                        can_add = combined.p.can_add,
                        can_delete = combined.p.can_delete,
                        can_edit = combined.p.can_edit,
                        can_print = combined.p.can_print,
                        can_download = combined.p.can_download,
                        can_upload = combined.p.can_upload,
                        can_execute = combined.p.can_execute,
                        can_authorize = combined.p.can_authorize,
                        can_view_sensitive = combined.p.can_view_sensitive,
                        can_export_data = combined.p.can_export_data,
                        can_import_data = combined.p.can_import_data,
                        can_approve_changes = combined.p.can_approve_changes
                    })
                    .GroupBy(pm => pm.menu_id)
                    .Select(g => new AuthenticatedMenuPermission
                    {
                        menu_id = g.Key,
                        menu_name = g.First().menu_name,
                        menu_path = g.First().menu_path,
                        can_view = g.Max(pm => pm.can_view),
                        can_add = g.Max(pm => pm.can_add),
                        can_delete = g.Max(pm => pm.can_delete),
                        can_edit = g.Max(pm => pm.can_edit),
                        can_print = g.Max(pm => pm.can_print),
                        can_download = g.Max(pm => pm.can_download),
                        can_upload = g.Max(pm => pm.can_upload),
                        can_execute = g.Max(pm => pm.can_execute),
                        can_authorize = g.Max(pm => pm.can_authorize),
                        can_view_sensitive = g.Max(pm => pm.can_view_sensitive),
                        can_export_data = g.Max(pm => pm.can_export_data),
                        can_import_data = g.Max(pm => pm.can_import_data),
                        can_approve_changes = g.Max(pm => pm.can_approve_changes)
                    })
                    .ToList();
                }

                return Ok(PermissionMenus, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region private logic
        protected async Task<string?> getImageUploadPath(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "UserProfile" && x.param_name == "BaseUploadPath").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = Path.Combine(result, lv1);
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = Path.Combine(result, lv2);
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = Path.Combine(result, lv3);
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = Path.Combine(result, lv4);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(result) && !Directory.Exists(result)) { Directory.CreateDirectory(result); }
            }
            return result;
        }

        protected async Task<string?> getImageViewUrl(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.app_system_params.Where(x => x.param_group == "UserProfile" && x.param_name == "ImageViewUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(lv1))
                {
                    result = result + "/" + lv1;
                    if (!string.IsNullOrEmpty(lv2))
                    {
                        result = result + "/" + lv2;
                        if (!string.IsNullOrEmpty(lv3))
                        {
                            result = result + "/" + lv3;
                            if (!string.IsNullOrEmpty(lv4))
                            {
                                result = result + "/" + lv4;
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region crud
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RegisterModel model)
        {
            try
            {
                var runUserID = await getDefRunUserId();
                var runUser = await getDefRunUser();

                #region validation
                var userExists = await _userManager.FindByNameAsync(model.Username.Trim(new char[] { (char)39 }).Replace(" ", ""));
                if (userExists != null) return Error(userExists, "Nama pengguna telah digunakan.");

                var IdExist = await _dbContext.Users.Where(x => x.IdNo == model.ICNo || x.Email == model.Email).Select(x => new { x.IdNo, x.Email }).AsNoTracking().FirstOrDefaultAsync();
                if (IdExist != null)
                {
                    if (IdExist.IdNo == model.ICNo)
                    {
                        return Error("", "Icno telah berdaftar dengan pengguna lain");
                    }

                    if (IdExist.Email == model.Email)
                    {
                        return Error("", "e-mel telah berdaftar dengan pengguna lain");
                    }
                    if (IdExist.IdNo == model.Username)
                    {
                        return Error("", "Nama pengguna telah berdaftar");
                    }
                }
                string dayOfBirth = model.ICNo.Substring(4,8);
                string firstTwoDigits = dayOfBirth.Substring(0, 2);  
                string lastTwoDigits = dayOfBirth.Substring(2, 2);

                string icLastDigits = model.ICNo.Substring(model.ICNo.Length - 4);

                model.Password = firstTwoDigits + lastTwoDigits + icLastDigits;

                var validationResult = await _passwordValidator.ValidateAsync(null, null, model.Password);

                if (!validationResult.Succeeded)
                {
                    return Error("", "Kata laluan tidak menepati kriteria.");
                }

                #endregion

                #region store data
                ApplicationUser au = new ApplicationUser
                {
                    full_name = model.FullName,
                    PhoneNumber = model.PhoneNo,
                    PhoneNumberConfirmed = true,
                    IdNo = model.ICNo,
                    Email = model.Email,
                    EmailConfirmed = true,
                    dept_id = model.DepartmentID,
                    div_id = model.DivisionID,
                    unit_id = model.UnitID,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username.Trim(new char[] { (char)39 }).Replace(" ", ""),
                    CreatedAt = DateTime.Now,
                    CreatorId = runUserID,
                    PhotoFilename = "",
                    PhotoPathUrl = "",
                    SignFilename = "",
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifierId = runUserID,
                };        
                #endregion

                var result = await _userManager.CreateAsync(au, model.Password);
                if (!result.Succeeded)
                {
                    return Error(result, "Gagal cipta pengguna.");
                }
                else
                {
                    await SendEmailCreateUser(model.Email, model.Username, model.FullName, model.Password);
                }

                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] RegisterModel model)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                /*
                var users = await (from user in _dbContext.Users
                                   join dept in _dbContext.ref_departments on user.dept_id equals dept.dept_id
                                   join div in _dbContext.ref_divisions on user.div_id equals div.div_id
                                   join unit in _dbContext.ref_units on user.unit_id equals unit.unit_id
                                   where user.IdNo == model.ICNo
                                   select new RegisterModel
                                   {
                                       Id = user.Id,
                                       FullName = user.full_name,
                                       Username = user.UserName,
                                       Email = user.Email,
                                       PhoneNo = user.PhoneNumber,
                                       DepartmentName = dept.dept_name,
                                       DivisionName = div.div_name,
                                       UnitName = unit.unit_name,
                                       ICNo = user.IdNo,
                                       Name = user.UserName,
                                       Password = user.PasswordHash,

                                   }).FirstOrDefaultAsync();
                */
                var users = await _dbContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }


                #endregion
                //ApplicationUser au = new ApplicationUser();
                users.full_name = model.FullName;
                users.IdNo = model.ICNo;
                users.PhoneNumber = model.PhoneNo;
                users.dept_id = model.DepartmentID;
                users.div_id = model.DivisionID;
                users.unit_id = model.UnitID;
                users.ModifiedAt = DateTime.Now;
                users.ModifierId = runUserID;

                _dbContext.Users.Update(users);
                await _dbContext.SaveChangesAsync();

                return Ok(users, SystemMesg(_feature, "Update", MessageTypeEnum.Success, string.Format("Berjaya mengubahsuai medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                int runUserID = await getDefRunUserId();

                #region Validation
                var users = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == Id);

                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                if (users != null)
                {
                    bool userHasRole = await _dbContext.UserRoles
                        .AnyAsync(ur => ur.UserId == users.Id);

                    if (userHasRole)
                    {
                        return Error("", SystemMesg(_feature, "", MessageTypeEnum.Error, string.Format("Pengguna masih mempunyai akses di Pengguna & Peranan.  ")));
                    }
                    else
                    {
                        users.IsDeleted = true;
                    }
                }
                #endregion

                try
                {
                    _dbContext.Users.Remove(users);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    users.IsDeleted = true;
                    users.ModifierId = runUserID;
                    users.ModifiedAt = DateTime.Now;

                    _dbContext.Users.Update(users);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                }

                return Ok(users, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetail(int Id)
        {
            try
            {
                var users = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == Id);

                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(users, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        private async Task<bool> SendEmailCreateUser(string recipient, string username, string fullname, string password)
        {
            try
            {
                EmailContent defaultContent = new EmailContent
                {
                    subject = "Nama pengguna dan Katalaluan anda.",
                    body = "Hai [0], berikut ialah maklumat nama pengguna dan katalaluan anda.<br/><br/>" +
                            "Nama pengguna: [1]<br/>" +
                            "Kata laluan: [2]<br/><br/>" +
                            "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir PBT Pro<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                };

                string[] param = { fullname, username, password };

                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("CREATE_USER", param, defaultContent);

                var emailRs = await emailHelper.QueueEmail(emailContent.subject, emailContent.body, recipient);
                var sentRs = await emailHelper.ForceProcessQueue(emailRs);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return false;
            }
        }

        [HttpGet("{icno}")]
        public async Task<IActionResult> RetrievebyIc(string icno)
        {
            try
            {
                var users = await _dbContext.Users.Where(x => x.IdNo == icno)
              .Select(x => new user_profile_view
              {
                  user_id = x.Id,
                  user_name = x.UserName,
                  unit_id = x.unit_id,
                  div_id = x.div_id,
                  dept_id = x.dept_id,
                  full_name = x.full_name,
                  idno = x.IdNo,
                  photo_filename = x.PhotoFilename,
                  sign_filename = x.SignFilename,
                  email = x.Email,
                  phone_number = x.PhoneNumber,
                  last_login = x.LastLogin,
              }).AsNoTracking().FirstOrDefaultAsync();

                users.dept_name = await _tenantDBContext.ref_departments.Where(x => x.dept_id == users.dept_id).Select(x => x.dept_name).FirstOrDefaultAsync();
                users.div_name = await _tenantDBContext.ref_divisions.Where(x => x.div_id == users.div_id).Select(x => x.div_name).FirstOrDefaultAsync();
                users.unit_name = await _tenantDBContext.ref_units.Where(x => x.unit_id == users.unit_id).Select(x => x.unit_name).FirstOrDefaultAsync();

                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                return Ok(users, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} Message : {1}, Inner Exception {2}", _feature, ex.Message, ex.InnerException));
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
    }
}
