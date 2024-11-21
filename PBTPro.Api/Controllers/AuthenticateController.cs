using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PBTPro.Api.Controllers.Base;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.DAL.Models.CommonServices;
using System.Security.Claims;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : IBaseController
    {
        private readonly ILogger<AuthenticateController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JWTTokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _feature = "AUTH";

        public AuthenticateController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, PBTProDbContext dbContext, ILogger<AuthenticateController> logger, JWTTokenService tokenService, IEmailSender emailSender) : base(dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("localregister")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            #region validation
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Error("", SystemMesg(_feature, "NAME_ISREQUIRED", MessageTypeEnum.Error, string.Format("Nama adalah wajib")));
            }

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

                ApplicationUser user = new ApplicationUser();
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.UserName = model.Username.ToLower();
                user.NormalizedUserName = model.Username.ToUpper();
                user.Name = model.Name;
                user.Status = "A";
                user.EmailConfirmed = true;
                user.PhoneNumber = model.PhoneNo;
                if (!string.IsNullOrEmpty(model.PhoneNo)) user.PhoneNumberConfirmed = true;
                user.CreatedBy = runUser;
                user.CreatedDtm = DateTime.Now;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return Error("", SystemMesg(_feature, "REGISTER", MessageTypeEnum.Error, string.Format("Gagal mencipta pengguna, sila hubungi pentadbir sistem atau cuba semula kemudian.")));
                }

                if (!string.IsNullOrWhiteSpace(user?.Email))
                {
                    await SendEmailSelfRegisterMember(user.Email, user.UserName, model.Password, model.Name);
                }

                return Error("", SystemMesg(_feature, "REGISTER", MessageTypeEnum.Success, string.Format("Pengguna berjaya dicipta.")));

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

                var LoginResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                if (LoginResult.Succeeded)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                    var roles = await _userManager.GetRolesAsync(user);

                    foreach (var role in roles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var token = _tokenService.GenerateJwtToken(authClaims, model.RememberMe);
                    return Ok(new LoginResult {Fullname = user.Name, Userid = user.Id, Username = user.UserName, Token = token, Roles = roles.ToList() }, SystemMesg(_feature, "LOGIN", MessageTypeEnum.Success, string.Format("Log masuk berjaya.")));
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
                    if (user.AccessFailedCount < 4)
                    {
                        var attempLeft = (5 - user.AccessFailedCount);
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

        #region Private Logic
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
        #endregion

    }
}
