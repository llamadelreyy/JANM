using Microsoft.AspNetCore.Http;

namespace PBTPro.DAL.Models.PayLoads
{
    public class patrol_cfsc_input_model
    {
        public string? owner_icno { get; set; }
        public string? tax_accno { get; set; }
        public string? cfsc_ref_no { get; set; }
        public string? instruction { get; set; }
        public string? offs_location { get; set; }
        public int? scen_id { get; set; }
        public decimal? cfsc_longitude { get; set; }
        public decimal? cfsc_latitude { get; set; }
        public int? trnstatus_id { get; set; }
        public int? license_id { get; set; }
        public int? inv_id { get; set; }
        public int? inv_type_id { get; set; }
        public string? offense_code { get; set; }
        public string? uuk_code { get; set; }
        public string? act_code { get; set; }
        public string? section_code { get; set; }
        public int? schedule_id { get; set; }
        public bool? is_tax { get; set; }
        public int? user_id { get; set; }
        public List<patrol_cfsc_witness>? witnesses { get; set; }
        public List<patrol_cfsc_item_model>? items { get; set; }
        public List<IFormFile>? proofs { get; set; }

        //2025-04-08 - added new field
        public string? recipient_name { get; set; }
        public string? recipient_icno { get; set; }
        public string? recipient_telno { get; set; }
        public string? recipient_addr { get; set; }
        public int? recipient_relation_id { get; set; }
        public IFormFile? recipient_sign { get; set; }
    }

    public class patrol_cfsc_witness
    {
        public int? user_id { get; set; }
        public string? name { get; set; }
    }

    public class patrol_cfsc_item_model
    {
        public int? inv_id { get; set; }
        public string? description { get; set; }
        public int? cnt_item { get; set; }
    }

    public class patrol_cfsc_view_model : trn_cfsc
    {
        public List<trn_cfsc_item>? items { get; set; }
        public List<trn_cfsc_img>? proofs { get; set; }
    }
}
