using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PBTPro.DAL.Models.CommonServices
{
    public class RegisterModel
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public string PhoneNo { get; set; }

        //added by farhana
        public string? ICNo { get; set; }
        public int DepartmentID { get; set; }
        public int DivisionID { get; set; }
        public int UnitID { get; set; }
        public string FullName { get; set; }
        public int IdTypeId { get; set; }
    }

    public class LoginResult
    {
        public int Userid { get; set; }
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Fullname { get; set; }
        public bool IsMobileUser { get; set; } = false;
        public List<string?> Roles { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Nama Pengguna diperlukan.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Kata Laluan diperlukan.")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordInput
    {
        [Required(ErrorMessage = "Nama Pengguna diperlukan.")]
        public string username { get; set; } = null!;

        [Required(ErrorMessage = "Password Baru diperlukan.")]
        public string new_password { get; set; } = null!;

        [Required(ErrorMessage = "Sahkan Password diperlukan.")]
        public string valid_new_password { get; set; } = null!;
        public string reset_token { get; set; }
    }

    public class AuthenticatedUser
    {
        public int Userid { get; set; } = 0;
        public string Fullname { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Token { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();

        public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(new Claim[]
        {
            new (ClaimTypes.Name, Username),
            new (ClaimTypes.Hash, Password),
            new ("AccessToken", Token)
        }.Concat(Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray()), "AUTH"));

        public static AuthenticatedUser FromClaimsPrincipal(ClaimsPrincipal principal) => new()
        {
            Username = principal.FindFirstValue(ClaimTypes.Name),
            Password = principal.FindFirstValue(ClaimTypes.Hash),
            Token = principal.FindFirstValue("AccessToken"),
            Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }

    public class AuthenticatedMenuPermission
    {
        public int menu_id { get; set; }
        public string menu_name { get; set; }
        public string menu_path { get; set; }
        public bool can_view { get; set; }
        public bool can_add { get; set; }
        public bool can_delete { get; set; }
        public bool can_edit { get; set; }
        public bool can_print { get; set; }
        public bool can_download { get; set; }
        public bool can_upload { get; set; }
        public bool can_execute { get; set; }
        public bool can_authorize { get; set; }
        public bool can_view_sensitive { get; set; }
        public bool can_export_data { get; set; }
        public bool can_import_data { get; set; }
        public bool can_approve_changes { get; set; }
    }

    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatorId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int ModifierId { get; set; }
        public bool IsDeleted { get; set; }
        //role_id
        public int RoleId { get; set; }
        //user_id
        public int UserId { get; set; }
        //user_name
        public string UserName { get; set; }
    }
    public class RoleModel
    {
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDesc { get; set; }
        public bool? IsDefaultRole { get; set; }
        public bool? IsTenant { get; set; }
    }
}
