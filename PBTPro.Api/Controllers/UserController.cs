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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf.Types;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Data;
using System.Security.Claims;

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
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                user_profile_view data = new user_profile_view();

                var baseImageViewURL = await getImageViewUrl();
                var AvatarViewURL = baseImageViewURL + "/profile";
                var SignatureViewURL = baseImageViewURL + "/signature";

                var UserProfile = await _dbContext.user_profiles.Where(x => x.profile_user_id == UserId).Select(x => new user_profile_view
                {
                    profile_id = x.profile_id,
                    profile_user_id = x.profile_user_id,
                    profile_photo_url = !string.IsNullOrWhiteSpace(x.profile_photo_filename) ? AvatarViewURL + "/" + x.profile_photo_filename : AvatarViewURL + "/avatar-user-profile-icon.jpg",
                    profile_name = x.profile_name,
                    profile_dob = x.profile_dob,
                    profile_icno = x.profile_icno,
                    profile_nat_id = x.profile_nat_id,
                    profile_race_id = x.profile_race_id,
                    profile_address1 = x.profile_address1,
                    profile_address2 = x.profile_address2,
                    profile_postcode = x.profile_postcode,
                    profile_city_id = x.profile_city_id,
                    profile_district_id = x.profile_district_id,
                    profile_state_id = x.profile_state_id,
                    profile_country_id = x.profile_country_id,
                    profile_accept_term1 = x.profile_accept_term1,
                    profile_accept_term2 = x.profile_accept_term2,
                    profile_email = x.profile_email,
                    profile_employee_no = "ABC9090112",
                    profile_department_view = "Penguatkuasa",
                    profile_section_view = "Operasi",
                    profile_unit_view = "Operasi & Penguatkuasa",
                    profile_signature_url = !string.IsNullOrWhiteSpace(x.profile_signature_filename) ? SignatureViewURL + "/" + x.profile_signature_filename : null
                }).AsNoTracking().FirstOrDefaultAsync();

                if(UserProfile == null)
                {
                    UserProfile = await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new user_profile_view
                    {
                        profile_user_id = x.Id,
                        profile_name = x.Name ?? x.UserName,
                        profile_email = x.Email,
                        profile_employee_no = "ABC9090112",
                        profile_department_view = "Penguatkuasa",
                        profile_section_view = "Operasi",
                        profile_unit_view = "Operasi & Penguatkuasa"
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
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(UserId != InputModel.user_id)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                user_profile? userProfile = await _dbContext.user_profiles.FirstOrDefaultAsync(x => x.profile_user_id == UserId);

                if(userProfile == null)
                {
                    isNew = true;
                    userProfile = await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new user_profile
                    {
                        profile_user_id = x.Id,
                        profile_name = x.Name ?? x.UserName,
                        profile_email = x.Email
                    }).AsNoTracking().FirstOrDefaultAsync();
                }
                #endregion

                string? FileName = userProfile.profile_signature_filename;
                //D:\Workspace\Dotnet\New\PBTPro.Server\wwwroot\images\signature
                if (InputModel.sign_image?.Length > 0)
                {
                    string ImageUploadExt = Path.GetExtension(InputModel.sign_image.FileName).ToString().ToLower();

                    FileName = UserId + ImageUploadExt;
                    var UploadPath = await getImageUploadPath("signature");
                    var Fullpath = Path.Combine(UploadPath, FileName);
                    using (var stream = new FileStream(Fullpath, FileMode.Create))
                    {
                        await InputModel.sign_image.CopyToAsync(stream);
                    }

                    userProfile.profile_signature_filename = FileName;
                    
                    if(isNew == true)
                    {
                        userProfile.created_by = runUserID;
                        userProfile.created_date = DateTime.Now;
                        _dbContext.user_profiles.Add(userProfile);
                    }
                    else
                    {
                        userProfile.updated_by = runUserID;
                        userProfile.updated_date = DateTime.Now;
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
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (UserId != InputModel.user_id)
                {
                    return Error("", SystemMesg(_feature, "INVALID_USERID", MessageTypeEnum.Error, string.Format("Rekod tidak sah")));
                }

                user_profile? userProfile = await _dbContext.user_profiles.FirstOrDefaultAsync(x => x.profile_user_id == UserId);

                if (userProfile == null)
                {
                    isNew = true;
                    userProfile = await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new user_profile
                    {
                        profile_user_id = x.Id,
                        profile_name = x.Name ?? x.UserName,
                        profile_email = x.Email
                    }).AsNoTracking().FirstOrDefaultAsync();
                }
                #endregion

                string? FileName = userProfile.profile_signature_filename;
                //D:\Workspace\Dotnet\New\PBTPro.Server\wwwroot\images\signature
                if (InputModel.avatar_image?.Length > 0)
                {
                    string ImageUploadExt = Path.GetExtension(InputModel.avatar_image.FileName).ToString().ToLower();

                    FileName = UserId + ImageUploadExt;
                    var UploadPath = await getImageUploadPath("profile");
                    var Fullpath = Path.Combine(UploadPath, FileName);
                    using (var stream = new FileStream(Fullpath, FileMode.Create))
                    {
                        await InputModel.avatar_image.CopyToAsync(stream);
                    }

                    userProfile.profile_photo_filename = FileName;

                    if (isNew == true)
                    {
                        userProfile.created_by = runUserID;
                        userProfile.created_date = DateTime.Now;
                        _dbContext.user_profiles.Add(userProfile);
                    }
                    else
                    {
                        userProfile.updated_by = runUserID;
                        userProfile.updated_date = DateTime.Now;
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

                if(InputModel.new_password != InputModel.valid_new_password)
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
                            if(error.Code.ToLower() == "passwordtooshort")
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

                    if(resultADD != null && resultADD.Succeeded)
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
    }
}
