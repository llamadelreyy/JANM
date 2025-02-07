using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class tenant
{
    public int tenant_id { get; set; }

    public string? tn_name { get; set; }

    public string? tn_email { get; set; }

    public string? tn_photo_filename { get; set; }

    public string? tn_photo_url { get; set; }

    public string? tn_doc_filename { get; set; }

    public string? tn_doc_url { get; set; }

    public string? addr_line1 { get; set; }

    public string? addr_line2 { get; set; }

    public string? town_code { get; set; }

    public string? district_code { get; set; }

    public string? state_code { get; set; }

    public string? country_code { get; set; }

    public string? postcode { get; set; }

    public string? phone_number { get; set; }

    public string? contact_name { get; set; }

    public bool? accept_term1 { get; set; }

    public bool? accept_term2 { get; set; }

    public bool? accept_term3 { get; set; }

    public bool? accept_term4 { get; set; }

    public string? site_name { get; set; }

    public string? connection_string { get; set; }

    public string? schema_name { get; set; }

    public string? table_prefix { get; set; }

    public string? recipe_name { get; set; }

    public string? tn_handle { get; set; }

    public string? url_prefix { get; set; }

    public string? website_link { get; set; }

    public string? confirm_website_link { get; set; }

    public int? website_status_id { get; set; }

    public int? subsc_status_id { get; set; }

    public DateOnly? subsc_start_date { get; set; }

    public DateOnly? subsc_end_date { get; set; }

    public DateOnly? reminder_date { get; set; }

    public DateTime? created_at { get; set; }

    public int? creator_id { get; set; }

    public DateTime? modified_at { get; set; }

    public int? modifier_id { get; set; }

    public bool? is_deleted { get; set; }
}
