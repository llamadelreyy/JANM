/*
Project: PBT Pro
Description: custom model for users
Author: ismail
Date: December 2024
Version: 1.0
Additional Notes:
- this model to override default dotnet identity user

Changes Logs:
30/12/2024 - initial create
*/
using Microsoft.AspNetCore.Identity;

namespace PBTPro.DAL
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? full_name { get; set; }
        public string? IdNo { get; set; }
        public int unit_id { get; set; }
        public int div_id { get; set; }
        public int dept_id { get; set; }
        public override string PasswordHash { get; set; }
        public string? Salt { get; set; }
        public DateTime? PwdUpdateAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? VerificationCode { get; set; }
        public override string SecurityStamp { get; set; }
        public string? PhotoFilename { get; set; } = "";
        public string? PhotoPathUrl { get; set; } = "";
        public string? SignFilename { get; set; } = "";
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatorId { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifierId { get; set; }

        public string Text => $"{UserName} ({full_name})";
    }
}
