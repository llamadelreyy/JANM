using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class UserProfile
{
    public int ProfileId { get; set; }

    public string ProfileName { get; set; } = null!;

    public string? ProfilePhotoName { get; set; }

    public string ProfileEmail { get; set; } = null!;

    public string? ProfileTelNo { get; set; }

    public string? ProfileIcno { get; set; }

    public DateOnly? ProfileDob { get; set; }

    public int? ProfileNatId { get; set; }

    public int? ProfileRaceId { get; set; }

    public string ProfileAddress1 { get; set; } = null!;

    public string ProfileAddress2 { get; set; } = null!;

    public int? ProfileCityId { get; set; }

    public int? ProfileDistrictId { get; set; }

    public int? ProfileStateId { get; set; }

    public int? ProfileCountryId { get; set; }

    public char? ProfilePostcode { get; set; }

    public char? ProfileAcceptTerm1 { get; set; }

    public char? ProfileAcceptTerm2 { get; set; }

    public char? ProfileAcceptTerm3 { get; set; }

    public string ProfileStatus { get; set; } = null!;

    public DateTime? ProfileLastLogin { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string? ProfileUserId { get; set; }

    public int? ProfileDepartmentId { get; set; }

    public int? ProfileSectionId { get; set; }

    public int? ProfileUnitId { get; set; }

    public string? ProfileUserName { get; set; }

    public string? ProfileStaffNo { get; set; }
    public string ProfileUserRoles { get; set; } = "";
    public string Text => $"{ProfileUserName} ({ProfileName})"; 
}
