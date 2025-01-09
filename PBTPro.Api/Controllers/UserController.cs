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

        public UserController(PBTProDbContext dbContext, ILogger<UserController> logger, UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> identityOptions) : base(dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _identityOptions = identityOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var users = await _dbContext.Users.AsNoTracking().ToListAsync();
                return Ok(users, SystemMesg(_feature, "LOAD_DATA", MessageTypeEnum.Success, string.Format("Senarai rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
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

                var UserProfile = await _dbContext.user_profiles.Where(x => x.user_id == UserId).Select(x => new user_profile_view
                {
                    profile_id = x.profile_id,
                    user_id = x.user_id.Value,
                    unit_code = x.unit_code,
                    div_code = x.div_code,
                    dept_code = x.dept_code,
                    nat_id = x.nat_id,
                    race_id = x.race_id,
                    gen_id = x.gen_id,
                    profile_name = x.profile_name,
                    profile_photoname = x.profile_photoname,
                    profile_email = x.profile_email,
                    profile_telno = x.profile_telno,
                    profile_icno = x.profile_icno,
                    profile_dob = x.profile_dob,
                    profile_postcode = x.profile_postcode,
                    profile_accept_term1 = x.profile_accept_term1,
                    profile_accept_term2 = x.profile_accept_term2,
                    profile_accept_term3 = x.profile_accept_term3,
                    profile_last_login = x.profile_last_login,
                    profile_signfile = x.profile_signfile,
                    profile_employee_no = "ABC9090112",
                    dept_name = "Penguatkuasa",
                    div_name = "Operasi",
                    unit_name = "Operasi & Penguatkuasa",
                    profile_photo_url = !string.IsNullOrWhiteSpace(x.profile_photoname) ? AvatarViewURL + "/" + x.profile_photoname : AvatarViewURL + "/avatar-user-profile-icon.jpg",
                    profile_signature_url = !string.IsNullOrWhiteSpace(x.profile_signfile) ? SignatureViewURL + "/" + x.profile_signfile : null
                }).AsNoTracking().FirstOrDefaultAsync();

                if (UserProfile == null)
                {
                    UserProfile = await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new user_profile_view
                    {
                        user_id = x.Id,
                        profile_name = x.UserName,
                        profile_email = x.Email,
                        profile_employee_no = "ABC9090112",
                        dept_name = "Penguatkuasa",
                        div_name = "Operasi",
                        unit_name = "Operasi & Penguatkuasa",
                        profile_photo_url = AvatarViewURL + "/avatar-user-profile-icon.jpg",
                    }).AsNoTracking().FirstOrDefaultAsync();
                }

                return Ok(UserProfile, SystemMesg(_feature, "VIEW_USER_PROFILE", MessageTypeEnum.Success, string.Format("Rekod berjaya dijana")));
            }
            catch (Exception ex)
            {
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

                user_profile? userProfile = await _dbContext.user_profiles.FirstOrDefaultAsync(x => x.user_id == runUserID);

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
                #endregion

                string? FileName = userProfile.profile_signfile;
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

                    userProfile.profile_signfile = FileName;

                    if (isNew == true)
                    {
                        userProfile.creator_id = runUserID;
                        userProfile.created_at = DateTime.Now;
                        _dbContext.user_profiles.Add(userProfile);
                    }
                    else
                    {
                        userProfile.modifier_id = runUserID;
                        userProfile.modified_at = DateTime.Now;
                        _dbContext.user_profiles.Update(userProfile);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "UPDATE_SIGNATURE", MessageTypeEnum.Success, string.Format("Tandatangan berjaya disimpan")));
            }
            catch (Exception ex)
            {
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

                user_profile? userProfile = await _dbContext.user_profiles.FirstOrDefaultAsync(x => x.user_id == runUserID);

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
                #endregion

                string? FileName = userProfile.profile_photoname;
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

                    userProfile.profile_photoname = FileName;

                    if (isNew == true)
                    {
                        userProfile.creator_id = runUserID;
                        userProfile.created_at = DateTime.Now;
                        _dbContext.user_profiles.Add(userProfile);
                    }
                    else
                    {
                        userProfile.modifier_id = runUserID;
                        userProfile.modified_at = DateTime.Now;
                        _dbContext.user_profiles.Update(userProfile);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("", SystemMesg(_feature, "UPDATE_AVATAR", MessageTypeEnum.Success, string.Format("Gambar profil berjaya disimpan")));
            }
            catch (Exception ex)
            {
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region private logic
        protected async Task<string?> getImageUploadPath(string? lv1 = null, string? lv2 = null, string? lv3 = null, string? lv4 = null)
        {
            string? result;

            using (PBTProDbContext _iwkContext = new PBTProDbContext())
            {
                result = await _dbContext.config_system_params.Where(x => x.param_group == "UserProfile" && x.param_name == "BaseUploadPath").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

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
                result = await _dbContext.config_system_params.Where(x => x.param_group == "UserProfile" && x.param_name == "ImageViewUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();

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
                }

                #endregion
                #region store data
                ApplicationUser au = new ApplicationUser
                {
                    full_name = model.FullName,
                    PhoneNumber = model.PhoneNo,
                    IdNo = model.ICNo,
                    IdTypeId = model.IdTypeId,
                    Email = model.Email,
                    dept_id = model.DepartmentID,
                    div_id = model.DivisionID,
                    unit_id = model.UnitID,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username.Trim(new char[] { (char)39 }).Replace(" ", ""),
                };

                _dbContext.Users.Add(au);
                await _dbContext.SaveChangesAsync();

                #endregion
                model.Password = GeneratePassword();
                var result = await _userManager.CreateAsync(au, model.Password);
                if (!result.Succeeded) return Error(result, "Gagal cipta pengguna.");

                return Ok(result, SystemMesg(_feature, "CREATE", MessageTypeEnum.Success, string.Format("Berjaya cipta jadual rondaan")));
            }
            catch (Exception ex)
            {
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
                var users = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdNo == model.ICNo);
                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                users.full_name = model.FullName;
                users.IdNo = model.ICNo;
                users.PhoneNumber = model.PhoneNo;
                users.IdTypeId = model.IdTypeId;
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                #region Validation
                var users = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == Id);
                if (users == null)
                {
                    return Error("", SystemMesg(_feature, "INVALID_RECID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }
                #endregion

                _dbContext.Users.Remove(users);
                await _dbContext.SaveChangesAsync();

                return Ok(users, SystemMesg(_feature, "REMOVE", MessageTypeEnum.Success, string.Format("Berjaya membuang medan")));
            }
            catch (Exception ex)
            {
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
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }
        #endregion

        public string GeneratePassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
            };

            Random rand = new Random(System.Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (opts.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (opts.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (opts.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
