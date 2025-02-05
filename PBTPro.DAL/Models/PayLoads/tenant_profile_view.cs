using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class tenant_profile_view
    {
        public int tenant_id { get; set; }

        public string tn_name { get; set; }

        public string? tn_photo_url { get; set; }

        public string? addr_line1 { get; set; }

        public string? addr_line2 { get; set; }

        public string? town_code { get; set; }

        public string? district_code { get; set; }

        public string? state_code { get; set; }

        public string? country_code { get; set; }

        public string? postcode { get; set; }
    }

    public class update_tenant_profile_input_model
    {
        public int tenant_id { get; set; }

        [Required(ErrorMessage = "Ruangan Name diperlukan.")]
        public string tn_name { get; set; } = null!;

        public IFormFile? tn_photo_file { get; set; }

        public string? addr_line1 { get; set; }

        public string? addr_line2 { get; set; }

        public string? town_code { get; set; }

        public string? district_code { get; set; }

        public string? state_code { get; set; }

        public string? country_code { get; set; }

        public string? postcode { get; set; }
    }
}
