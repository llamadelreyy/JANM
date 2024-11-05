using Microsoft.AspNetCore.Identity;

namespace PBTPro.DAL
{
    public class ApplicationUser : IdentityUser
    {
        public string? NetworkId { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDtm { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? OfficePhone { get; set; }
        public DateTime? LastSeenDtm { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDtm { get; set; }
        public string? UnitOffice { get; set; }
        public string? Department { get; set; }
        public string? LoginKey { get; set; }
    }
}
