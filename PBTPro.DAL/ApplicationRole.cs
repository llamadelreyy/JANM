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
        //role_id
        public int Id { get; set; }
        //role_name
        public string Name { get; set; }
        //role_desc
        public string RoleDesc { get; set; }
        //is_default_role
        public bool IsDefaultRole { get; set; }
        //is_tenant
        public bool IsTenant { get; set; }
        //is_deleted
        public bool IsDeleted { get; set; }
        //created_at
        public DateTime CreatedAt { get; set; }
        //creator_id
        public int CreatorId { get; set; }
        //modified_at
        public DateTime? ModifiedAt { get; set; }
        //modifier_id
        public int? ModifierId { get; set; }
    }        
}
