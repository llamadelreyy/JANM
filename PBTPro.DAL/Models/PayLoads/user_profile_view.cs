using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class user_profile_view
    {
        public int profile_id { get; set; }

        public string profile_user_id { get; set; } = null!;

        public string profile_name { get; set; } = null!;
        
        public string? profile_photo_url { get; set; }

        public string profile_email { get; set; } = null!;

        public string? profile_tel_no { get; set; }

        public string? profile_icno { get; set; }

        public DateOnly? profile_dob { get; set; }

        public int? profile_nat_id { get; set; }

        public int? profile_race_id { get; set; }

        public string profile_address1 { get; set; } = null!;

        public string profile_address2 { get; set; } = null!;

        public int? profile_city_id { get; set; }

        public int? profile_district_id { get; set; }

        public int? profile_state_id { get; set; }

        public int? profile_country_id { get; set; }

        public char? profile_postcode { get; set; }

        public char? profile_accept_term1 { get; set; }

        public char? profile_accept_term2 { get; set; }

        public char? profile_accept_term3 { get; set; }

        public string profile_status { get; set; } = null!;

        public DateTime? profile_last_login { get; set; }

        public int? profile_department_id { get; set; }

        public int? profile_section_id { get; set; }

        public int? profile_unit_id { get; set; }

        //Virtual

        public string? profile_city_view { get; set; }

        public string? profile_district_view { get; set; }

        public string? profile_state_view { get; set; }

        public string? profile_country_view { get; set; }

        public string? profile_employee_no { get; set; }

        public string? profile_department_view { get; set; }

        public string? profile_section_view { get; set; }

        public string? profile_unit_view { get; set; }

        public string? profile_signature_url { get; set; }
    }

    public class update_signature_input_model
    {
        public string user_id { get; set; } = null!;
        public IFormFile? sign_image { get; set; }

    }

    public class update_avatar_input_model
    {
        public string user_id { get; set; } = null!;
        public IFormFile? avatar_image { get; set; }

    }
}
