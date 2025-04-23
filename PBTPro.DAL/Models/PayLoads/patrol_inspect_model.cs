using Microsoft.AspNetCore.Http;

namespace PBTPro.DAL.Models.PayLoads
{
    public class patrol_inspect_input_model
    {
        public string? owner_icno { get; set; }
        public string? inspect_ref_no { get; set; }
        public string? notes { get; set; }
        public string? offs_location { get; set; }
        public int? dept_id { get; set; }
        public decimal? inspect_longitude { get; set; }
        public decimal? inspect_latitude { get; set; }
        public int? trnstatus_id { get; set; }
        public int? license_id { get; set; }
        public int? schedule_id { get; set; }
        public string? tax_accno { get; set; }
        public bool? is_tax { get; set; }
        public int? user_id { get; set; }
        public List<patrol_inspect_witness>? witnesses { get; set; }
        public List<IFormFile>? proofs { get; set; }
    }

    public class patrol_inspect_witness
    {
        public int? user_id { get; set; }
        public string? name { get; set; }
    }

    public class patrol_inspect_view_model : trn_inspect
    {
        public List<trn_inspect_img>? proofs { get; set; }
    }
}
