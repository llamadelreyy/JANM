using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models;

public partial class user_profile
{
    public int profile_id { get; set; }

    public int profile_user_id { get; set; }

    public string profile_name { get; set; } = null!;

    public string? profile_photo_filename { get; set; }

    [Required(ErrorMessage = "Medan Emel perlu diisi.")]
    public string profile_email { get; set; } = null!;


    [Required(ErrorMessage = "Medan No Telefon perlu diisi.")]
    public string? profile_tel_no { get; set; }

    public string? profile_icno { get; set; }

    public DateOnly? profile_dob { get; set; }

    public int? profile_nat_id { get; set; }

    public int? profile_race_id { get; set; }

    public string? profile_address1 { get; set; }

    public string? profile_address2 { get; set; }

    public int? profile_city_id { get; set; }

    public int? profile_district_id { get; set; }

    public int? profile_state_id { get; set; }

    public int? profile_country_id { get; set; }

    public char? profile_postcode { get; set; }

    public char? profile_accept_term1 { get; set; }

    public char? profile_accept_term2 { get; set; }

    public char? profile_accept_term3 { get; set; }

    public string? profile_status { get; set; }

    public DateTime? profile_last_login { get; set; }

    public DateTime? created_date { get; set; }

    public int? created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public int? updated_by { get; set; }

    public int profile_department_id { get; set; }
    public string profile_department_name { get; set; } = null!;
    public int profile_section_id { get; set; }
    public string profile_section_name { get; set; } = null!;
    public int profile_unit_id { get; set; }
    public string profile_unit_name { get; set; } = null!;
    public int profile_role_id { get; set; }
    public string profile_role { get; set; } = null!;

    public string userOldPassword { get; set; } = "";
    public int? profile_unit_id { get; set; }

    public string? profile_signature_filename { get; set; }
}
