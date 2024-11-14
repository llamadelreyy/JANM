using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PBT.Data;

public class UserProp
{
    public int user_id { get; set; }
    [Required]
    public string user_name { get; set; } = "";
    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Minimum Katalaluan mesti 6 huruf atau lebih.")]
    [MaxLength(20)]
    public string user_password { get; set; } = "";
    [Required]
    public string user_old_password { get; set; } = "";

    public int dept_id { get; set; }
    public string dept_name { get; set; } = "";
    public string phone_no { get; set; } = "";

    public string staff_no { get; set; } = "";
    [Required]
    [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Alamat emel tidak sah.")]
    public string user_email { get; set; } = "";
    public string user_key { get; set; } = "";
    [Required]
    [RegularExpression(@"^[^0-9]+$", ErrorMessage = "Do not use digits in the Name.")]
    [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "Nama pengguna mesti sekurang-kurangnya 2 abjad.")]
    [MaxLength(20, ErrorMessage = "Text is too long")]
    public string user_full_name { get; set; } = "";
    public string Text => $"{user_name} ({user_full_name})";

    [Required]
    public string user_roles { get; set; } = "";
    public int wrong_login_attempt { get; set; }
    public int today_login_attempt { get; set; }

    public DateTime? created_date { get; set; }
    public DateTime? updated_date { get; set; }
    public string created_by { get; set; }
    public string updated_by { get; set; }

    public string active_flag { get; set; } = "";
    public List<string> roles { get; set; } = new();
    //////public int Age { get; set; }
    public string ErrorUserMsg { get; set; } = "";


    public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(new Claim[]
    {
        //////new (ClaimTypes.Name, Username),
        //////new (ClaimTypes.Hash, Password),
        //////new (nameof(Age), Age.ToString())
        new (ClaimTypes.Name, user_name),
        new (ClaimTypes.Hash, user_password)
    }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray()), "PBTPRO"));

    public static UserProp FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        //////Username = principal.FindFirstValue(ClaimTypes.Name),
        //////Password = principal.FindFirstValue(ClaimTypes.Hash),
        //////Age = Convert.ToInt32(principal.FindFirstValue(nameof(Age))),
        //////Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ///
        user_name = principal.FindFirstValue(ClaimTypes.Name),
        user_password = principal.FindFirstValue(ClaimTypes.Hash),
        roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
    };
}