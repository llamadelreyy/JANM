using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PBT.Data;

public class UserProp
{
    public int UserId { get; set; }
    [Required]
    public string userName { get; set; } = "";
    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Minimum Katalaluan mesti 6 huruf atau lebih.")]
    [MaxLength(20)]
    public string userPassword { get; set; } = "";
    [Required]
    public string userOldPassword { get; set; } = "";

    public int dept_id { get; set; }
    public string dept_name { get; set; } = "";
    public string phone_no { get; set; } = "";

    public string staff_no { get; set; } = "";
    [Required]
    [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Alamat emel tidak sah.")]
    public string userEmail { get; set; } = "";
    public string userKey { get; set; } = "";
    [Required]
    [RegularExpression(@"^[^0-9]+$", ErrorMessage = "Do not use digits in the Name.")]
    [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "Nama pengguna mesti sekurang-kurangnya 2 abjad.")]
    [MaxLength(20, ErrorMessage = "Text is too long")]
    public string userFullName { get; set; } = "";
    [Required]
    public string userRoles { get; set; } = "";
    public int wrongLoginAttempt { get; set; }
    public int todayLoginAttempt { get; set; }

    public DateTime? created_date { get; set; }
    public DateTime? updated_date { get; set; }
    public string created_by { get; set; }
    public string updated_by { get; set; }

    public string rekStatus { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    //////public int Age { get; set; }
    public string ErrorUserMsg { get; set; } = "";


    public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(new Claim[]
    {
        //////new (ClaimTypes.Name, Username),
        //////new (ClaimTypes.Hash, Password),
        //////new (nameof(Age), Age.ToString())
        new (ClaimTypes.Name, userName),
        new (ClaimTypes.Hash, userPassword)
    }.Concat(Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray()), "JAINJ"));

    public static UserProp FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        //////Username = principal.FindFirstValue(ClaimTypes.Name),
        //////Password = principal.FindFirstValue(ClaimTypes.Hash),
        //////Age = Convert.ToInt32(principal.FindFirstValue(nameof(Age))),
        //////Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ///
        userName = principal.FindFirstValue(ClaimTypes.Name),
        userPassword = principal.FindFirstValue(ClaimTypes.Hash),
        Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
    };
}