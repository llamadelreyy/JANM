using System.ComponentModel.DataAnnotations;

namespace PBTPro.Shared.Models.CommonService
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
        public string Token { get; set; } = null!;
        public List<string?> Roles { get; set; }

    }

    public class LoginModel
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
