using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Shape;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.DAL.Models.PayLoads
{
    public class app_marking_tax_input_model
    {
        public string? owner_name { get; set; }
        public string? owner_icno { get; set; }
        public string? owner_phone_no { get; set; }
        public string? owner_address { get; set; }
        public string? tax_acc_no { get; set; }
        public string? tax_lot_no { get; set; }
        public string? tax_address { get; set; }
        public string? tax_notes { get; set; }
        public string? tax_category { get; set; }
        public IFormFile? tax_image { get; set; }
        public int? tax_status { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }

    public class app_marking_license_input_model
    {
        public string? codeid_premis { get; set; }
        public string? owner_name { get; set; }
        public string? owner_icno { get; set; }
        public string? owner_phone_no { get; set; }
        public string? owner_address { get; set; }
        public string? ssm_no { get; set; }
        public string? business_name { get; set; }
        public string? business_address { get; set; }
        public string? license_acc_no { get; set; }
        public int? license_type { get; set; }
        public int? license_status { get; set; }
        public string? license_activity { get; set; }
        public string? license_floor { get; set; }
        public string? license_notes { get; set; }
        public string? license_pic_name { get; set; }
        public string? license_pic_phone_no { get; set; }
        public List<IFormFile>? license_images { get; set; }
    }

    public class app_marking_marker
    {
        public string? codeid_premis { get; set; }
        public string? lot { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class app_marking_view : app_marking_tax
    {
        public List<app_marking_license> licensees { get; set; }
    }

    public class app_marking_tax
    {
        public int? id { get; set; }
        public string? codeid_premis { get; set; }
        public string? owner_name { get; set; }
        public string? owner_icno { get; set; }
        public string? owner_phone_no { get; set; }
        public string? owner_address { get; set; }
        public string? tax_acc_no { get; set; }
        public int? tax_status { get; set; }
        public string? lot { get; set; }
        public string? address { get; set; }
        public string? notes { get; set; }
        public string? category { get; set; }
        public string? image { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? status { get; set; }
    }

    public class app_marking_license
    {
        public int? id { get; set; }
        public string? codeid_premis { get; set; }
        public string? owner_name { get; set; }
        public string? owner_icno { get; set; }
        public string? owner_phone_no { get; set; }
        public string? owner_address { get; set; }
        public string? ssm_no { get; set; }
        public string? business_name { get; set; }
        public string? business_address { get; set; }
        public string? license_acc_no { get; set; }
        public int? license_type { get; set; }
        public int? license_status { get; set; }
        public string? activity { get; set; }
        public string? floor { get; set; }
        public string? notes { get; set; }
        public string? pic_name { get; set; }
        public string? pic_phone_no { get; set; }
        public string? image_1 { get; set; }
        public string? image_2 { get; set; }
        public string? image_3 { get; set; }
        public string? image_4 { get; set; }
        public string? image_5 { get; set; }
        public string? image_6 { get; set; }
        public string? status { get; set; }

    }

}
