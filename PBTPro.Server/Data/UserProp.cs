using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PBT.Data;

public class UserProp
{
    public int userID { get; set; }
    [Required]
    public string userName { get; set; } = "";

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Minimum Katalaluan mesti 6 huruf atau lebih.")]
    [MaxLength(20)]
    public string userPassword { get; set; } = "";

    public string userDepartment { get; set; } = "";

    [Required]
    public string userOldPassword { get; set; } = "";

    [Required(ErrorMessage = "Medan Katalaluan lama perlu diisi.")]
    [Compare("userOldPassword", ErrorMessage = "Katalaluan yang dimasukkan tidak sama dengan katalaluan yang asal.")]
    public string userNowPassword { get; set; } = "";

    [Required, RegularExpression(@"^(?=.*\d)(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;""'<>,.?/_])(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage = "Kata laluan tidak sah!")]
    [DataType(DataType.Password)]
    [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara campuran abjad, nombor dan aksara khas.", MinimumLength = 8)]
    public string userNewPassword { get; set; } = "";

    [Required(ErrorMessage = "Medan Sahkan Katalaluan perlu diisi.")]
    [DataType(DataType.Password)]
    [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara.", MinimumLength = 8)]
    [Compare("userNewPassword", ErrorMessage = "Katalaluan yang dimasukkan tidak sama.")]
    public string userConfirmPassword { get; set; } = "";

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
    public int SuperUser { get; set; }
    public int SuperExec { get; set; }
    public DateTime? rekCipta { get; set; }
    public int rekCiptaUserID { get; set; }
    public DateTime? rekUbah { get; set; }
    public int rekUbahUserID { get; set; }
    public string rekStatus { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    //////public int Age { get; set; }
    public string ErrorUserMsg { get; set; } = "";


    public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(new Claim[]
    {
        new (ClaimTypes.Name, userName),
        new (ClaimTypes.Hash, userPassword)
    }.Concat(Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray()),"PBTPRO"));

    public static UserProp FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        userName = principal.FindFirstValue(ClaimTypes.Name),
        userPassword = principal.FindFirstValue(ClaimTypes.Hash),
        Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
    };
}