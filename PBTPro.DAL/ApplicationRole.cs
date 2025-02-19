/*
Project: PBT Pro
Description: custom model for roles
Author: ismail
Date: December 2024
Version: 1.0
Additional Notes:
- this model to override default dotnet identity role

Changes Logs:
30/12/2024 - initial create
*/
using Microsoft.AspNetCore.Identity;

namespace PBTPro.DAL
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string RoleDesc { get; set; }
        public bool IsDefaultRole { get; set; }
        public bool role_select { get; set; }
        //is_deleted
        public bool IsDeleted { get; set; }
        public bool IsMobileUser { get; set; }
        public string? DefaultPage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatorId { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifierId { get; set; }
    }
}
