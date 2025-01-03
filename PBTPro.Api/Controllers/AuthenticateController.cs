using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PBTPro.Api.Constants;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using System.Security.Claims;
using System.Text;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : IBaseController
    {
        private readonly ILogger<AuthenticateController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityOptions _identityOptions;
        private readonly IConfiguration _configuration;
        private readonly JWTTokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly string _feature = "AUTH";

        public AuthenticateController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<IdentityOptions> identityOptions, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, PBTProDbContext dbContext, ILogger<AuthenticateController> logger, JWTTokenService tokenService, IEmailSender emailSender) : base(dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _identityOptions = identityOptions.Value;
        }

        [HttpPost]
        [Route("localregister")]
        [Authorize(Roles = "Administrator, Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            #region validation
            //if (string.IsNullOrWhiteSpace(model.Name))
            //{
            //    return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Nama adalah wajib")));
            //}

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return Error("", SystemMesg(_feature, "USERNAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Nama Pengguna adalah wajib")));
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return Error("", SystemMesg(_feature, "EMAIL_ISREQUIRED", MessageTypeEnum.Error, string.Format("E-mel adalah wajib")));
            }

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return Error("", SystemMesg(_feature, "USER_ISEXISTS", MessageTypeEnum.Error, string.Format("Nama pengguna telah digunakan")));
            }

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                return Error("", SystemMesg(_feature, "EMAIL_ISEXISTS", MessageTypeEnum.Error, string.Format("E-mel telah digunakan")));
            }
            #endregion
            try
            {
                var runUser = await getDefRunUser();
                var runUserId = await getDefRunUserId();

                ApplicationUser user = new ApplicationUser();
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.UserName = model.Username.ToLower();
                //user.LoginKey = user.UserName;
                //user.NormalizedUserName = model.Username.ToUpper();
                //user.Name = model.Name;
                //user.Status = "A";
                user.EmailConfirmed = true;
                user.PhoneNumber = model.PhoneNo;
                if (!string.IsNullOrEmpty(model.PhoneNo)) user.PhoneNumberConfirmed = true;
                user.CreatorId = runUserId;
                user.ModifierId = runUserId;
                user.CreatedAt = DateTime.Now;
                user.ModifiedAt = DateTime.Now;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    List<string> passwordErrors = new List<string>();
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

                    if (passwordErrors.Count > 0)
                    {
                        var ValidationErr = String.Join("\r\n- ", passwordErrors.ToList());
                        List<string> param = new List<string> { "- " + ValidationErr };
                        return Error("", SystemMesg(_feature, "INVALID_PASS_COMBINATION", MessageTypeEnum.Error, string.Format("Kombinasi katalaluan tidak diterima :\r\n[0]"), param));
                    }
                    else
                    {
                        return Error("", SystemMesg(_feature, "REGISTER", MessageTypeEnum.Error, string.Format("Gagal mencipta pengguna, sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.Name))
                {
                    user_profile up = new user_profile
                    {
                        user_id = user.Id,
                        profile_name = model.Name,
                        profile_email = user.Email,
                        profile_icno = user.IdNo,
                        profile_telno = user.PhoneNumber,
                        creator_id = runUserId,
                        created_at = DateTime.Now,
                        modifier_id = runUserId,
                        modified_at = DateTime.Now
                    };

                    _dbContext.user_profiles.Add(up);
                    await _dbContext.SaveChangesAsync();
                }

                if (!string.IsNullOrWhiteSpace(user?.Email))
                {
                    await SendEmailSelfRegisterMember(user.Email, user.UserName, model.Password, model.Name);
                }

                return Ok("", SystemMesg(_feature, "REGISTER", MessageTypeEnum.Success, string.Format("Pengguna berjaya dicipta.")));

            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return Error("", SystemMesg(_feature, "USER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pengguna tidak sah.")));
                }
                
                string Fullname = user.UserName;

                var LoginResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                if (LoginResult.Succeeded)
                {
                    user.LastLogin = DateTime.Now;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                    var roles = await _userManager.GetRolesAsync(user);

                    foreach (var role in roles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var userProfile = await _dbContext.user_profiles.AsNoTracking().FirstOrDefaultAsync(x => x.user_id == user.Id);
                    if(userProfile != null)
                    {
                        Fullname = userProfile.profile_name;
                    }

                    var token = _tokenService.GenerateJwtToken(authClaims, model.RememberMe);
                    return Ok(new LoginResult {Fullname = user.UserName, Userid = user.Id, Username = user.UserName, Token = token, Roles = roles.ToList() }, SystemMesg(_feature, "LOGIN", MessageTypeEnum.Success, string.Format("Log masuk berjaya.")));
                }
                else if (LoginResult.IsLockedOut)
                {
                    DateTimeOffset? lockoutEnd = user.LockoutEnd;
                    if (lockoutEnd.HasValue)
                    {
                        DateTime currentTime = DateTime.Now;
                        TimeSpan difference = lockoutEnd.Value - currentTime;

                        string Message = "";
                        if(difference.Days > 0)
                        {
                            Message += $" {difference.Days} hari";
                        }
                        if (difference.Hours > 0)
                        {
                            Message += $" {difference.Hours} jam";
                        }
                        if (difference.Minutes > 0)
                        {
                            Message += $" {difference.Minutes} minit";
                        }
                        if (difference.Seconds > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(Message))
                            {
                                Message += " dan";
                            }

                            Message += $" {difference.Seconds} saat";
                        }

                        List<string> param = new List<string> { Message };
                        return Error("", SystemMesg(_feature, "RETRY_AFTER", MessageTypeEnum.Error, string.Format("Percubaan log masuk maksimum dicapai. Sila cuba selepas[0]"), param));
                    }
                    return Error("", SystemMesg(_feature, "MAXED_ATTEMPT", MessageTypeEnum.Error, string.Format("Percubaan log masuk maksimum dicapai. Hubungi pentadbir sistem untuk menyahsekat")));
                }
                else
                {
                    if (user.AccessFailedCount < 2)
                    {
                        var attempLeft = (3 - user.AccessFailedCount);
                        List<string> param = new List<string> { attempLeft.ToString() };
                        return Error("", SystemMesg(_feature, "INCORRECT_LOGIN", MessageTypeEnum.Error, string.Format("Kata laluan tidak sah. [0] cubaan tinggal."), param));
                    }
                    else
                    {
                        return Error("", SystemMesg(_feature, "INCORRECT_LAST_TRY", MessageTypeEnum.Error, string.Format("Kata laluan tidak sah. cubaan terakhir.")));
                    }
                }
            }
            catch(Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string Username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(Username);
                if (user == null)
                {
                    return Error("", SystemMesg(_feature, "USER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pengguna tidak sah.")));
                }

                string UIPublicUrl = await getBaseUIPublicURL();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var endpoint = new Uri(new Uri(UIPublicUrl), "/reset_password").ToString();

                var resetPasswordUrl = endpoint + "?token=" + token;
                await SendEmailForgotPassword(user.Email, user.UserName, resetPasswordUrl);
                
                return Ok("", SystemMesg(_feature, "FORGOT_PASSWORD", MessageTypeEnum.Success, string.Format("Berjaya menjana pautan terlupa kata laluan.")));
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordInput model)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(model.new_password))
                {
                    return Error("", SystemMesg(_feature, "NEW_PASSWORD_ISNULL", MessageTypeEnum.Error, string.Format("Kata Laluan baharu diperlukan")));
                }

                if (string.IsNullOrWhiteSpace(model.valid_new_password))
                {
                    return Error("", SystemMesg(_feature, "VALID_NEW_PASS_ISNULL", MessageTypeEnum.Error, string.Format("Sahkan Kata Laluan diperlukan")));
                }

                if (string.IsNullOrWhiteSpace(model.reset_token))
                {
                    return Error("", SystemMesg(_feature, "INVALID_RESET_PASSWORD_TOKEN", MessageTypeEnum.Error, string.Format("Token tidak sah.")));
                }

                if (model.new_password != model.valid_new_password)
                {
                    return Error("", SystemMesg(_feature, "NEW_PASS_MISSMATCH", MessageTypeEnum.Error, string.Format("Kata Laluan baharu tidak sepadan")));
                }

                var user = await _userManager.FindByNameAsync(model.username);
                if (user == null)
                {
                    return Error("", SystemMesg(_feature, "USER_NOT_EXISTS", MessageTypeEnum.Error, string.Format("Pengguna tidak sah.")));
                }

                byte[] tokenData = Convert.FromBase64String(model.reset_token);
                string decodedToken = System.Text.Encoding.UTF8.GetString(tokenData);
                if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", decodedToken))
                {
                    return Error("", SystemMesg(_feature, "INVALID_RESET_PASSWORD_TOKEN", MessageTypeEnum.Error, string.Format("Token tidak sah.")));
                }

                List<string> passwordErrors = new List<string>();
                var validators = _userManager.PasswordValidators;
                foreach (var validator in validators)
                {
                    var result = await validator.ValidateAsync(_userManager, null, model.new_password);
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

                var resultd = await _userManager.ResetPasswordAsync(user, decodedToken, model.new_password);
                if (resultd.Succeeded)
                {
                    return Ok("", SystemMesg(_feature, "RESET_PASSWORD", MessageTypeEnum.Success, string.Format("Berjaya menetap semula kata laluan")));
                }
                else
                {
                    return Error("", SystemMesg(_feature, "RESET_PASSWORD", MessageTypeEnum.Error, string.Format("Gagal menetap semula kata laluan")));
                }
            }
            catch (Exception ex)
            {
                return Error("", SystemMesg("COMMON", "UNEXPECTED_ERROR", MessageTypeEnum.Error, string.Format("Maaf berlaku ralat yang tidak dijangka. sila hubungi pentadbir sistem atau cuba semula kemudian.")));
            }
        }

        #region Private Logic
        private async Task<string?> getBaseUIPublicURL()
        {
            string? result = null;

            try
            {
                result = await _dbContext.config_system_params.Where(x => x.param_group == "Core" && x.param_name == "BaseUIPublicUrl").Select(x => x.param_value).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            return result;
        }

        private async Task<bool> SendEmailSelfRegisterMember(string recipient, string username, string password, string name)
        {
            try
            {
                //Default Email Template
                EmailContent defaultContent = new EmailContent
                {
                    subject = "Selamat datang ke PBTPro",
                    body = "Assalamualaikum [0], Selamat datang ke PBTPro.<br/><br/>" +
                    "Anda telah berjaya mendaftarkan akaun anda di PBTPro.<br/><br/>" +
                    "Berikut ialah maklumat log masuk anda : <br/> <strong>Nama Pengguna : </strong> [1]<br/> <strong>Katalaluan : </strong> [2] <strong><br/><br/>" +
                    "Terima Kasih.<br/><br/>Salam sejahtera,<br/>Pentadbir PBTPro<br/><br/><i>**Ini adalah mesej automatik, sila jangan balas**</i>",
                };

                string[] param = { name, username, password };

                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("NEW_SELFREGISTER_USER", param, defaultContent);

                var emailRs = await emailHelper.QueueEmail(emailContent.subject, emailContent.body, recipient);
                var sentRs = await emailHelper.ForceProcessQueue(emailRs);
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> SendEmailForgotPassword(string recipient, string username, string resetPasswordUrl)
        {
            try
            {
                //Default Email Template
                EmailContent defaultContent = new EmailContent
                {
                    subject = "Set semula kata laluan",
                    body = "Hai [0], Anda telah mohon set semula kata laluan.<br/><br/>" +
                    "Untuk set semula kata laluan anda sila klik pada pautan berikut: <a href=\"[1]\">Set Semula Kata Laluan</a><br/><br/>" +
                    "Jika anda tidak memohon set semula kata laluan, sila abaikan emel ini.<br/><br/>" +
                    "Terima Kasih.<br/><br/>Yang benar,<br/>Pentadbir MasjidKita<br/><br/><i>**Ini adalah mesej automatik. sila jangan balas**</i>",
                };

                string[] param = { username, resetPasswordUrl };

                var emailHelper = new EmailHelper(_dbContext, _emailSender);
                EmailContent emailContent = await emailHelper.getEmailContent("FORGOT_PASSWORD", param, defaultContent);

                var emailRs = await emailHelper.QueueEmail(emailContent.subject, emailContent.body, recipient);
                var sentRs = await emailHelper.ForceProcessQueue(emailRs);     
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

    }
}
