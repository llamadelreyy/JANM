using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class user_profile
{
    public int profile_id { get; set; }

    public int? user_id { get; set; }

    public string? unit_code { get; set; }

    public string? div_code { get; set; }

    public string? dept_code { get; set; }

    public int? nat_id { get; set; }

    public int? race_id { get; set; }

    public int? gen_id { get; set; }

    public string profile_name { get; set; } = null!;

    public string? profile_photoname { get; set; }

    public string profile_email { get; set; } = null!;

    public string? profile_telno { get; set; }

    public string? profile_icno { get; set; }

    public DateOnly? profile_dob { get; set; }

    public string? profile_postcode { get; set; }

    public bool? profile_accept_term1 { get; set; }

    public bool? profile_accept_term2 { get; set; }

    public bool? profile_accept_term3 { get; set; }

    public DateTime? profile_last_login { get; set; }

    public string? profile_signfile { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    //public int dept_id { get; set; }

    //public string dept_name { get; set; }

    //public int div_id { get; set; }

    //public string div_name { get; set; }

    //public int unit_id { get; set; }

    //public string unit_name { get; set; }


}
