using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{

    public class general_search_result_view
    {
        public int gid { get; set; }
        public JsonDocument? geom { get; set; }
        public string lot { get; set; }
        public string no_lesen { get; set; }
        public string no_cukai { get; set; }
        public string status_lesen { get; set; }
        public string status_cukai { get; set; }
        public string nama_perniagaan { get; set; }
        public string nama_pemilik { get; set; }
        public string alamat_premis { get; set; }
        public string icno_pemilik { get; set; }
    }

    public partial class general_search_premis_detail
    {
        public string? codeid_premis { get; set; }
        public int? taxholder_id { get; set; }
        public string? tax_accno { get; set; }
        public int? license_id { get; set; }
        public string? license_accno { get; set; }
        public string? premis_floor { get; set; }
        public string? premis_lot { get; set; }
        public string? premis_gkeseluruh { get; set; }
        public string? premis_category { get; set; }
        public double? premis_longitude { get; set; }
        public double? premis_latitude { get; set; }
        public int? tax_status_id { get; set; }
        public string? tax_status_view { get; set; }
        public string? tax_state_code { get; set; }
        public string? tax_district_code { get; set; }
        public int? tax_town_id { get; set; }
        public int? tax_parliment_id { get; set; }
        public int? tax_dun_id { get; set; }
        public int? tax_zon_id { get; set; }
        public string? tax_address { get; set; }
        public DateOnly? tax_start_date { get; set; }
        public DateOnly? tax_end_date { get; set; }
        public double? tax_total_amount { get; set; }
        public string? tax_owner_icno { get; set; }
        public string? tax_owner_name { get; set; }
        public string? tax_owner_email { get; set; }
        public string? tax_owner_telno { get; set; }
        public string? tax_owner_state_code { get; set; }
        public string? tax_owner_disctict_code { get; set; }
        public int? tax_owner_town_id { get; set; }
        public string? tax_owner_addess { get; set; }
        public int? license_status_id { get; set; }
        public string? license_status_view { get; set; }
        public string? license_ssmno { get; set; }
        public string? license_business_name { get; set; }
        public string? license_business_address { get; set; }
        public string? license_state_code { get; set; }
        public string? license_district_code { get; set; }
        public int? license_town_id { get; set; }
        public int? license_mukim_id { get; set; }
        public string? license_lot { get; set; }
        public string? license_duration { get; set; }
        public int? license_cat_id { get; set; }
        public string? license_type { get; set; }
        public double? license_total_amount { get; set; }
        public DateOnly? license_start_date { get; set; }
        public DateOnly? license_end_date { get; set; }
        public int? license_total_signboard { get; set; }
        public int? license_signboard_size { get; set; }
        public string? license_doc_support { get; set; }
        public string? license_g_activity_1 { get; set; }
        public string? license_g_activity_2 { get; set; }
        public string? license_g_activity_3 { get; set; }
        public string? license_g_signbboard_1 { get; set; }
        public string? license_g_signbboard_2 { get; set; }
        public string? license_g_signbboard_3 { get; set; }
        public string? license_owner_icno { get; set; }
        public string? license_owner_name { get; set; }
        public string? license_owner_email { get; set; }
        public string? license_owner_telno { get; set; }
        public string? license_owner_state_code { get; set; }
        public string? license_owner_disctict_code { get; set; }
        public int? license_owner_town_id { get; set; }
        public string? license_owner_addess { get; set; }
    }
}
