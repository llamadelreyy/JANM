/*
Project: PBT Pro
Description: custom model for user role link
Author: ismail
Date: December 2024
Version: 1.0
Additional Notes:
- this model to override default dotnet identity user role link

Changes Logs:
30/12/2024 - initial create
*/
using Microsoft.AspNetCore.Identity;

namespace PBTPro.DAL
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public int UserRoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatorId { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifierId { get; set; }
        public bool IsDeleted { get; set; }
        //role_id
        public int RoleId { get; set; }        
        //user_id
        public int UserId { get; set; }       
    }
}
