using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PBTPro.DAL.Models.CommonServices
{
    public class RegisterModel
    {
        public string? Name { get; set; } = null!;
        public string? Username { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; } = null!;

        public string? PhoneNo { get; set; }
    }

    public class LoginResult
    {
        public string Userid { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Fullname { get; set; }
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
        public string Userid { get; set; } = "";
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
}
