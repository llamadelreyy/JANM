using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.Shared.Models.CommonService;
using PBTPro.Shared.Models.RequestPayLoad;
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
                UserProfileViewModel data = new UserProfileViewModel();
                
                var UserProfile = await _dbContext.UserProfiles.Where(x => x.ProfileUserId == UserId).Select(x => new UserProfileViewModel
                {
                    ProfileId = x.ProfileId,
                    UserId = x.ProfileUserId,
                    PhotoUrl = !string.IsNullOrWhiteSpace(x.ProfilePhotoName) ? "/api/Files/GetProfileImage?fn=" + x.ProfilePhotoName : null,
                    Name = x.ProfileName,
                    Dob = x.ProfileDob,
                    Icno = x.ProfileIcno,
                    NatId = x.ProfileNatId,
                    RaceId = x.ProfileRaceId,
                    AddrLine1 = x.ProfileAddress1,
                    AddrLine2 = x.ProfileAddress2,
                    PostCode = x.ProfilePostcode,
                    CityId = x.ProfileCityId,
                    DistrictId = x.ProfileDistrictId,
                    StateId = x.ProfileStateId,
                    CountryId = x.ProfileCountryId,
                    AcceptTerms1 = x.ProfileAcceptTerm1,
                    AcceptTerms2 = x.ProfileAcceptTerm2,
                    Email = x.ProfileEmail,
                    EmployeeNo = "ABC9090112",
                    DepartmentView = "Penguatkuasa",
                    SectionView = "Operasi",
                    UnitView = "Operasi & Penguatkuasa"
                }).AsNoTracking().FirstOrDefaultAsync();

                if(UserProfile == null)
                {
                    UserProfile = await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new UserProfileViewModel
                    {
                        UserId = x.Id,
                        Name = x.Name ?? x.UserName,
                        Email = x.Email,
                        EmployeeNo = "ABC9090112",
                        DepartmentView = "Penguatkuasa",
                        SectionView = "Operasi",
                        UnitView = "Operasi & Penguatkuasa"
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
