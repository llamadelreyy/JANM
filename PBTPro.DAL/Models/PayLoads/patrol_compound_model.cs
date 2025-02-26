using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class patrol_cmpd_input_model
    {
        public string? owner_icno { get; set; }
        public string? cmpd_ref_no { get; set; }
        public string? instruction { get; set; }
        public string? offs_location { get; set; }
        public decimal? amt_cmpd { get; set; }
        public int? deliver_id { get; set; }
        public decimal? cmpd_longitude { get; set; }
        public decimal? cmpd_latitude { get; set; }
        public int? trnstatus_id { get; set; }
        public int? license_id { get; set; }
        public string? offense_code { get; set; }
        public string? uuk_code { get; set; }
        public string? act_code { get; set; }
        public string? section_code { get; set; }
        public int? schedule_id { get; set; }
        public string? tax_accno { get; set; }
        public bool? is_tax { get; set; }
        public int? user_id { get; set; }
        public List<IFormFile>? proofs { get; set; }
    }


}
