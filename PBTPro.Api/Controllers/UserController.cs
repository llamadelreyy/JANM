using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
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
        private readonly string _feature = "USER";
        public UserController(PBTProDbContext dbContext, ILogger<UserController> logger) : base(dbContext)
        {
            _logger = logger;
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
                
                var UserProfile = await _dbContext.user_profiles.Where(x => x.profile_user_id == UserId).Select(x => new user_profile_view
                {
                    profile_id = x.profile_id,
                    profile_user_id = x.profile_user_id,
                    profile_photo_url = !string.IsNullOrWhiteSpace(x.profile_photo_name) ? "/api/Files/GetProfileImage?fn=" + x.profile_photo_name : null,
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
                    profile_unit_view = "Operasi & Penguatkuasa"
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
    }
}
